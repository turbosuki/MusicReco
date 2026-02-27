using MusicReco.Domain;

namespace MusicReco;

internal sealed class RecommendationEngine(ILastFmClient lastFmClient) : IRecommendationEngine
{
    public async Task<List<AlbumRecommendation>> GetRecommendations(string artist, int count, int bias)
    {
        var similarArtists = await lastFmClient.GetSimilarArtists(artist, limit: 30);
        if (similarArtists.Count == 0)
        {
            Console.WriteLine("😿 Last.fm returned no similar artists for this one. Try another track.");
            return [];
        }

        var rng = Random.Shared;
        similarArtists = similarArtists.OrderBy(_ => rng.Next()).ToList();

        var recommendations = new List<AlbumRecommendation>();
        var usedArtists = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var similar in similarArtists)
        {
            if (!usedArtists.Add(similar))
                continue;

            var albums = await lastFmClient.GetTopAlbums(similar, limit: 12);
            if (albums.Count == 0)
                continue;

            var slice = albums.Take(Math.Min(bias, albums.Count)).ToList();
            var album = slice[rng.Next(slice.Count)];

            recommendations.Add(new AlbumRecommendation(similar, album));
            if (recommendations.Count >= count)
                break;

            await Task.Delay(100);
        }

        return recommendations;
    }
}
