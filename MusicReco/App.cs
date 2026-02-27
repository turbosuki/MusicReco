using MusicReco.Cli;
using MusicReco.Domain;

namespace MusicReco;

internal sealed class App(
    INowPlayingProvider nowPlayingProvider,
    IApiKeyProvider apiKeyProvider,
    IRecommendationEngineFactory recommendationEngineFactory,
    IUrlOpener urlOpener)
{
    public async Task<int> Run(string[] args)
    {
        var options = CliOptions.Parse(args);

        try
        {
            var nowPlaying = nowPlayingProvider.GetNowPlaying();
            Console.WriteLine($"🎧 Now playing: {nowPlaying.Track} - {nowPlaying.Artist}");

            var apiKey = apiKeyProvider.GetLastFmApiKey();
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("No Last.fm API key found. Store it in Keychain (recommended) or set LASTFM_API_KEY.");

            var engine = recommendationEngineFactory.Create(apiKey);
            var recs = await engine.GetRecommendations(nowPlaying.Artist, options.Count, options.Bias);

            if (recs.Count == 0)
            {
                Console.WriteLine("😿 Couldn’t find any albums to recommend. Try a different artist.");
                return 1;
            }

            Console.WriteLine();
            Console.WriteLine("✨ Album recommendations:");
            for (var i = 0; i < recs.Count; i++)
            {
                var rec = recs[i];
                Console.WriteLine($"{i + 1,2}. {rec.Artist} — {rec.Album}");
            }

            if (options.Open)
            {
                var first = recs[0];
                var url = LastFmLinks.BuildAlbumUrl(first.Artist, first.Album);
                Console.WriteLine();
                Console.WriteLine($"🌐 Opening: {url}");
                urlOpener.OpenUrl(url);
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"⚠️  {ex.Message}");
            return 1;
        }
    }
}
