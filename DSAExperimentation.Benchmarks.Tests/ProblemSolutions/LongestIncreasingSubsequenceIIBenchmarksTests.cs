using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestIncreasingSubsequenceIIBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the textbook O(n^2) dynamic program against
// this repo's segment tree keyed by value - so a harness whose arms disagree is timing two
// different problems. Both arms return the subsequence length, a scalar compared directly. This
// class carries no tuned property: the harness is built from a bare initializer plus Setup, whose
// fixed seed is the whole workload, so the same Length must rebuild the same values.
public sealed partial class LongestIncreasingSubsequenceIIBenchmarksTests
{
    private const int SmallestLength = 2_000;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().SegmentTreeValueWindow(),
            BuildHarness().SegmentTreeValueWindow());

    [Fact]
    public void DynamicProgramming_SmallestLength_AgreesWithSegmentTreeValueWindow()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SegmentTreeValueWindow(), harness.DynamicProgramming());
    }

    [Fact]
    public void SegmentTreeValueWindow_SmallestLength_AgreesWithDynamicProgramming()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DynamicProgramming(), harness.SegmentTreeValueWindow());
    }

    private static LongestIncreasingSubsequenceIIBenchmarks BuildHarness()
    {
        var harness = new LongestIncreasingSubsequenceIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
