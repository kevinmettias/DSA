using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumCostToBuyApplesIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - one per-source Dijkstra per shop against a reduced
// graph - so a harness whose arms disagree is timing two different networks. Answers come back one
// per shop in shop order, and that order is part of the answer: the cost at index i belongs to shop
// i, so AnswerText.Of and not OfUnorderedSet is the rendering that keeps each cost scored against its
// own shop. Setup builds the road network from one seeded stream, so the same ShopCount must rebuild
// the same network.
public sealed partial class MinimumCostToBuyApplesIIBenchmarksTests
{
    private const int SmallestShopCount = 30;

    [Fact]
    public void Setup_SameShopCount_RebuildsTheSameNetwork() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceDijkstra()),
            AnswerText.Of(BuildHarness().BruteForceDijkstra()));

    [Fact]
    public void BruteForceDijkstra_SeededRoadNetwork_AgreesWithReduceGraph()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.ReduceGraph()), AnswerText.Of(harness.BruteForceDijkstra()));
    }

    [Fact]
    public void ReduceGraph_SeededRoadNetwork_AgreesWithBruteForceDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForceDijkstra()), AnswerText.Of(harness.ReduceGraph()));
    }

    private static MinimumCostToBuyApplesIIBenchmarks BuildHarness()
    {
        var harness = new MinimumCostToBuyApplesIIBenchmarks { ShopCount = SmallestShopCount };
        harness.Setup();

        return harness;
    }
}
