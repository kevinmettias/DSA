using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TwoSumIVInputIsABSTBenchmarks (ARCHITECTURE 17.9): its two arms are
// TwoSumIVInputIsABSTSolution's competing strategies for the same question - the nested pair scan
// over the tree against one depth-first walk carrying a set of visited values - so a harness whose
// arms disagree is timing two different questions.
//
// The target is deliberately unreachable (every node value is non-negative, the target is -1), the
// same TwoSumBenchmarks convention, so both arms answer false on every workload this harness can
// build. The agreement below therefore witnesses that the two arms reach the same verdict on the
// same tree, not that either found a pair: both [Benchmark] return types are bool, so the pair is
// not observable from here, and tightening that is a harness decision rather than this file's.
public sealed partial class TwoSumIVInputIsABSTBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] node counts.
    private const int SmallestNodeCount = 200;

    // Setup's documented outcome: no two non-negative node values sum to the negative target.
    private const bool ExpectedHasPair = false;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(
            BuildHarness().HasTargetPairByNestedPairScan(),
            BuildHarness().HasTargetPairByNestedPairScan());
        Assert.Equal(ExpectedHasPair, BuildHarness().HasTargetPairByNestedPairScan());
    }

    [Fact]
    public void HasTargetPairByNestedPairScan_SmallestNodeCount_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasTargetPairByDepthFirstSetLookup(), harness.HasTargetPairByNestedPairScan());
        Assert.Equal(ExpectedHasPair, harness.HasTargetPairByNestedPairScan());
    }

    [Fact]
    public void HasTargetPairByDepthFirstSetLookup_SmallestNodeCount_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasTargetPairByNestedPairScan(), harness.HasTargetPairByDepthFirstSetLookup());
        Assert.Equal(ExpectedHasPair, harness.HasTargetPairByDepthFirstSetLookup());
    }

    private static TwoSumIVInputIsABSTBenchmarks BuildHarness()
    {
        var harness = new TwoSumIVInputIsABSTBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
