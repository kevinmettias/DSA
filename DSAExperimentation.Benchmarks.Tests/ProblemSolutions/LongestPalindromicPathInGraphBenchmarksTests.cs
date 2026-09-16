using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestPalindromicPathInGraphBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - enumerating simple paths by brute-force DFS
// against the bitmask memo - so a harness whose arms disagree is timing two different problems.
// Both arms return the palindromic path's node count, a scalar compared directly. Setup builds
// the labelled graph from a fixed seed through LabeledGraphWorkloads, so the same NodeCount must
// rebuild the same edges and labels; the answer depends on those draws, so the arms are compared
// against each other alone.
public sealed partial class LongestPalindromicPathInGraphBenchmarksTests
{
    private const int SmallestNodeCount = 8;

    [Fact]
    public void Setup_SmallestNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().BitmaskMemo(),
            BuildHarness().BitmaskMemo());

    [Fact]
    public void BruteForceDfs_SmallestNodeCount_AgreesWithBitmaskMemo()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BitmaskMemo(), harness.BruteForceDfs());
    }

    [Fact]
    public void BitmaskMemo_SmallestNodeCount_AgreesWithBruteForceDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceDfs(), harness.BitmaskMemo());
    }

    private static LongestPalindromicPathInGraphBenchmarks BuildHarness()
    {
        var harness = new LongestPalindromicPathInGraphBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
