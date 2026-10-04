using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PopulatingNextRightPointersInEachNodeIIBenchmarks (ARCHITECTURE 17.9). Arm
// agreement is BenchmarkArmsTests' job; this pins the full readout from the fixture's construction.
// Setup builds a right-only chain whose i-th node holds i mod 101, one node per level, so
// LeetCode's readout is each node's value followed by '#' (null): every level ends where it starts.
public sealed partial class PopulatingNextRightPointersInEachNodeIIBenchmarksTests
{
    private const int SmallestNodeCount = 200;
    private const int ValueSpan = 101;

    [Fact]
    public void ManualQueueBfs_RightChain_ReadsOneNodePerLevel() =>
        Assert.Equal(ExpectedReadout(), BuildHarness().ManualQueueBfs());

    [Fact]
    public void LevelGroupedTraversal_RightChain_ReadsOneNodePerLevel() =>
        Assert.Equal(ExpectedReadout(), BuildHarness().LevelGroupedTraversal());

    private static int?[] ExpectedReadout() =>
        [.. Enumerable.Range(0, SmallestNodeCount).SelectMany(index => new int?[] { index % ValueSpan, null })];

    private static PopulatingNextRightPointersInEachNodeIIBenchmarks BuildHarness()
    {
        var harness = new PopulatingNextRightPointersInEachNodeIIBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
