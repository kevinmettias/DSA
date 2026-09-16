using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumCostPathWithAlternatingDirectionsIIIBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - a hand-rolled BCL
// PriorityQueue + Dictionary Dijkstra over the (row, column, direction) state space against
// ShortestPath.Dijkstra composed with AlternatingGridTopology's on-the-fly edges - so a harness whose
// arms disagree is timing two different state spaces. Both arms answer with a long total cost, which
// they compare directly. Setup draws the penalty grid from one seeded stream, so the same Side must
// rebuild the same grid.
public sealed partial class MinimumCostPathWithAlternatingDirectionsIIIBenchmarksTests
{
    private const int SmallestSide = 30;

    [Fact]
    public void Setup_SameSide_RebuildsTheSamePenaltyGrid() =>
        Assert.Equal(BuildHarness().BclDijkstra(), BuildHarness().BclDijkstra());

    [Fact]
    public void BclDijkstra_RandomPenaltyGrid_AgreesWithStateDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StateDijkstra(), harness.BclDijkstra());
    }

    [Fact]
    public void StateDijkstra_RandomPenaltyGrid_AgreesWithBclDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BclDijkstra(), harness.StateDijkstra());
    }

    private static MinimumCostPathWithAlternatingDirectionsIIIBenchmarks BuildHarness()
    {
        var harness = new MinimumCostPathWithAlternatingDirectionsIIIBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
