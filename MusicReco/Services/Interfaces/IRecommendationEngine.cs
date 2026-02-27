using MusicReco.Domain;

namespace MusicReco;

internal interface IRecommendationEngine
{
    Task<List<AlbumRecommendation>> GetRecommendations(string artist, int count, int bias);
}
