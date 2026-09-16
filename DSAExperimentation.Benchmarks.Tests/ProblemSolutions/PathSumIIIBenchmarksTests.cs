using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PathSumIIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// PathSumIIISolution's, competing counts for the same question - a depth-first search restarted at
// every node against one prefix-sum walk - so a harness whose arms disagree is timing two
// different problems. Setup builds the balanced heap-layout tree from NodeCount alone, so the same
// NodeCount must rebuild the same tree.
//
// The agreement alone would be weak here: the target is deliberately unreachable (every node value
// in that tree is non-negative, so no downward path can sum to a negative target) and both arms
// therefore answer zero. The tests below assert that zero against a named constant as well, so
// agreement on a shared wrong answer could not pass.
public sealed partial class PathSumIIIBenchmarksTests
{
    private const int SmallestNodeCount = 200;
    private const int ExpectedMatchingPathCount = 0;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DoubleDfs(), BuildHarness().DoubleDfs());

    [Fact]
    public void DoubleDfs_BalancedNonNegativeTree_AgreesWithPrefixSumHashMap()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMatchingPathCount, harness.PrefixSumHashMap());
        Assert.Equal(harness.PrefixSumHashMap(), harness.DoubleDfs());
    }

    [Fact]
    public void PrefixSumHashMap_BalancedNonNegativeTree_AgreesWithDoubleDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMatchingPathCount, harness.DoubleDfs());
        Assert.Equal(harness.DoubleDfs(), harness.PrefixSumHashMap());
    }

    private static PathSumIIIBenchmarks BuildHarness()
    {
        var harness = new PathSumIIIBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
