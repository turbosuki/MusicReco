namespace MusicReco;

internal interface IRecommendationEngineFactory
{
    IRecommendationEngine Create(string apiKey);
}
