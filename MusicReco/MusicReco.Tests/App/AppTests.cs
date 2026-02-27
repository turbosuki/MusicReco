using MusicReco.Domain;
using MusicReco.Tests.Fakes;

namespace MusicReco.Tests.App;

public sealed class AppTests
{
    [Fact]
    public async Task Run_ReturnsSuccess_AndOpensUrl_WhenOpenFlagProvided()
    {
        var nowPlaying = new NowPlaying("Song", "Artist A");
        var engine = new FakeRecommendationEngine(
        [
            new AlbumRecommendation("Reco Artist", "Reco Album")
        ]);
        var factory = new FakeRecommendationEngineFactory(engine);
        var urlOpener = new FakeUrlOpener();

        var app = new global::MusicReco.App(
            new FakeNowPlayingProvider(nowPlaying),
            new FakeApiKeyProvider("api-key"),
            factory,
            urlOpener);

        var result = await app.Run(["--count", "3", "--bias", "4", "--open"]);

        Assert.Equal(0, result);
        Assert.Equal("api-key", factory.LastApiKey);
        Assert.Equal("Artist A", engine.LastArtist);
        Assert.Equal(3, engine.LastCount);
        Assert.Equal(4, engine.LastBias);
        Assert.Equal("https://www.last.fm/music/Reco%20Artist/Reco%20Album", urlOpener.OpenedUrl);
    }

    [Fact]
    public async Task Run_ReturnsError_WhenNoApiKey()
    {
        var app = new global::MusicReco.App(
            new FakeNowPlayingProvider(new NowPlaying("Song", "Artist A")),
            new FakeApiKeyProvider(""),
            new FakeRecommendationEngineFactory(new FakeRecommendationEngine([])),
            new FakeUrlOpener());

        var result = await app.Run([]);

        Assert.Equal(1, result);
    }
}
