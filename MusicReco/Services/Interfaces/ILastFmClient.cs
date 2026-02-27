namespace MusicReco;

internal interface ILastFmClient
{
    Task<List<string>> GetSimilarArtists(string artist, int limit);
    Task<List<string>> GetTopAlbums(string artist, int limit);
}
