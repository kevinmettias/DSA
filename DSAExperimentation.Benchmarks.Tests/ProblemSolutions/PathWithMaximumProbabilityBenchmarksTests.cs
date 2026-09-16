using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PathWithMaximumProbabilityBenchmarks (ARCHITECTURE 17.9): its two arms are
// PathWithMaximumProbabilitySolution's, competing searches for the same maximum success
// probability - an exhaustive walk over every source-to-target path against Dijkstra run over the
// edges reweighted to -log(probability) - so a harness whose arms disagree is timing two different
// problems. Setup builds the probability graph from one fixed seed, so the same NodeCount must
// rebuild the same graph, and both arms read that one graph and the same endpoints.
//
// The two arms reach the same product by different arithmetic - one multiplies probabilities
// along a path, the other exponentiates a summed log - so they agree only up to floating-point
// rounding and the comparison is made against a named relative tolerance rather than exactly.
public sealed partial class PathWithMaximumProbabilityBenchmarksTests
{
    private const double RelativeTolerance = 1e-9;

    private const int SmallestNodeCount = 10;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().ExhaustiveDfsOverEveryPath(),
            BuildHarness().ExhaustiveDfsOverEveryPath(),
            RelativeTolerance);

    [Fact]
    public void ExhaustiveDfsOverEveryPath_SeededProbabilityGraph_AgreesWithDijkstraOverNegativeLogWeights()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.DijkstraOverNegativeLogWeights(),
            harness.ExhaustiveDfsOverEveryPath(),
            RelativeTolerance);
    }

    [Fact]
    public void DijkstraOverNegativeLogWeights_SeededProbabilityGraph_AgreesWithExhaustiveDfsOverEveryPath()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.ExhaustiveDfsOverEveryPath(),
            harness.DijkstraOverNegativeLogWeights(),
            RelativeTolerance);
    }

    private static PathWithMaximumProbabilityBenchmarks BuildHarness()
    {
        var harness = new PathWithMaximumProbabilityBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
