using MusicReco;

static class Program
{
    public static async Task<int> Main(string[] args)
    {
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

        var app = new App(
            new MusicNowPlayingProvider(),
            new ApiKeyProvider(),
            new RecommendationEngineFactory(http),
            new MacUrlOpener());

        return await app.Run(args);
    }
}
