using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DigitOperationsToMakeTwoIntegersEqualBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - a textbook BCL priority-queue Dijkstra
// generating mutations on the fly against this repo's own ShortestPath.Dijkstra over the
// pre-built DigitStepGraph - so a harness whose arms disagree is timing two different problems.
// Setup pins each digit count to the widest composite endpoint pair LC 3377 admits, and 10 (the
// two-digit start) is composite, so a real cost exists and can never fall below the start value
// itself: LC 3377 charges the sum of every value the walk passes through, startValue included.
// Both endpoints and the graph are rebuilt from the same DigitCount, so the same DigitCount must
// rebuild the same workload.
public sealed partial class DigitOperationsToMakeTwoIntegersEqualBenchmarksTests
{
    private const int SmallestDigitCount = 2;
    private const int TwoDigitStartValue = 10;

    [Fact]
    public void Setup_CompositeEndpointPair_CostsAtLeastTheStartValueAndRebuildsTheSameWorkload()
    {
        var harness = BuildHarness();
        var operations = harness.BruteForceDijkstra();

        Assert.True(operations >= TwoDigitStartValue);
        Assert.Equal(operations, BuildHarness().BruteForceDijkstra());
    }

    [Fact]
    public void BruteForceDijkstra_CompositeEndpointPair_AgreesWithDijkstraOverDigitGraph()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DijkstraOverDigitGraph(), harness.BruteForceDijkstra());
    }

    [Fact]
    public void DijkstraOverDigitGraph_CompositeEndpointPair_AgreesWithBruteForceDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceDijkstra(), harness.DijkstraOverDigitGraph());
    }

    private static DigitOperationsToMakeTwoIntegersEqualBenchmarks BuildHarness()
    {
        var harness = new DigitOperationsToMakeTwoIntegersEqualBenchmarks { DigitCount = SmallestDigitCount };
        harness.Setup();

        return harness;
    }
}
