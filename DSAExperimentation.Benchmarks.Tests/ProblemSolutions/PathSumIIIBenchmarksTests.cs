using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PathSumIIIBenchmarks (ARCHITECTURE 17.9): the baseline's count against the one
// the workload's shape fixes, which the generic check cannot supply - it holds the prefix-sum walk to
// the baseline, and agreement on a shared wrong answer would pass it. The target is deliberately
// unreachable (every node value in the balanced heap-layout tree is non-negative, so no downward path
// can sum to a negative target), so the count is zero.
public sealed partial class PathSumIIIBenchmarksTests
{
    private const int SmallestNodeCount = 200;
    private const int ExpectedMatchingPathCount = 0;

    [Fact]
    public void DoubleDfs_BalancedNonNegativeTree_CountsNoPath() =>
        Assert.Equal(ExpectedMatchingPathCount, BuildHarness().DoubleDfs());

    private static PathSumIIIBenchmarks BuildHarness()
    {
        var harness = new PathSumIIIBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
