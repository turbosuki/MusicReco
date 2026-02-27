using MusicReco.Services.Implementations;

namespace MusicReco;

internal sealed class RecommendationEngineFactory(HttpClient httpClient) : IRecommendationEngineFactory
{
    public IRecommendationEngine Create(string apiKey)
    {
        var lastFmClient = new LastFmClient(httpClient, apiKey);
        return new RecommendationEngine(lastFmClient);
    }
}
