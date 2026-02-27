using MusicReco;
using MusicReco.Domain;
using MusicReco.Tests.Fakes;

namespace MusicReco.Tests.Services;

public sealed class RecommendationEngineTests
{
    [Fact]
    public async Task GetRecommendations_ReturnsAtMostRequestedCount()
    {
        var lastFm = new FakeLastFmClient(
            ["Artist1", "Artist2", "Artist3"],
            new Dictionary<string, List<string>>
            {
                ["Artist1"] = ["Album1", "Album2"],
                ["Artist2"] = ["Album3"],
                ["Artist3"] = ["Album4"]
            });

        var engine = new RecommendationEngine(lastFm);

        var recs = await engine.GetRecommendations("Source Artist", count: 2, bias: 2);

        Assert.Equal(2, recs.Count);
        Assert.All(recs, rec => Assert.False(string.IsNullOrWhiteSpace(rec.Album)));
    }

    [Fact]
    public async Task GetRecommendations_ReturnsEmpty_WhenNoSimilarArtists()
    {
        var lastFm = new FakeLastFmClient([], new Dictionary<string, List<string>>());
        var engine = new RecommendationEngine(lastFm);

        var recs = await engine.GetRecommendations("Source Artist", count: 5, bias: 3);

        Assert.Empty(recs);
        Assert.Equal(1, lastFm.SimilarCalls);
    }
}
