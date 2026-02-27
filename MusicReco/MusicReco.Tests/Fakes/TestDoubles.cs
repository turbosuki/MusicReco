using MusicReco;
using MusicReco.Domain;

namespace MusicReco.Tests.Fakes;

internal sealed class FakeNowPlayingProvider(NowPlaying nowPlaying) : INowPlayingProvider
{
    public NowPlaying GetNowPlaying() => nowPlaying;
}

internal sealed class FakeApiKeyProvider(string apiKey) : IApiKeyProvider
{
    public string GetLastFmApiKey() => apiKey;
}

internal sealed class FakeRecommendationEngine(List<AlbumRecommendation> recommendations) : IRecommendationEngine
{
    public string? LastArtist { get; private set; }
    public int LastCount { get; private set; }
    public int LastBias { get; private set; }

    public Task<List<AlbumRecommendation>> GetRecommendations(string artist, int count, int bias)
    {
        LastArtist = artist;
        LastCount = count;
        LastBias = bias;
        return Task.FromResult(recommendations);
    }
}

internal sealed class FakeRecommendationEngineFactory(IRecommendationEngine engine) : IRecommendationEngineFactory
{
    public string? LastApiKey { get; private set; }

    public IRecommendationEngine Create(string apiKey)
    {
        LastApiKey = apiKey;
        return engine;
    }
}

internal sealed class FakeUrlOpener : IUrlOpener
{
    public string? OpenedUrl { get; private set; }

    public void OpenUrl(string url)
    {
        OpenedUrl = url;
    }
}

internal sealed class FakeLastFmClient(
    List<string> similarArtists,
    Dictionary<string, List<string>> albumsByArtist) : ILastFmClient
{
    public int SimilarCalls { get; private set; }

    public Task<List<string>> GetSimilarArtists(string artist, int limit)
    {
        SimilarCalls++;
        return Task.FromResult(similarArtists);
    }

    public Task<List<string>> GetTopAlbums(string artist, int limit)
    {
        albumsByArtist.TryGetValue(artist, out var albums);
        return Task.FromResult(albums ?? []);
    }
}
