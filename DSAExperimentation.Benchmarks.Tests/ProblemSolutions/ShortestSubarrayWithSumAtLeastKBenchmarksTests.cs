using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortestSubarrayWithSumAtLeastKBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - a brute-force prefix scan against a
// monotonic deque sweep - so a harness whose arms disagree is scanning two different arrays.
// Setup draws the values from one fixed seed, so the same Length must rebuild the same array;
// otherwise two published numbers were never comparable.
//
// This class's agreement is weak by construction and is reported as such. K is deliberately
// far above any sum the bounded values can reach, so every arm is forced through its full
// worst-case scan and both return the same -1 the problem reserves for "no such subarray".
// The agreement therefore witnesses that both arms found no valid subarray - and the explicit
// check below that they return that sentinel, rather than a length, is what keeps the pair
// from passing on a shared answer neither arm actually computed. It does not witness that the
// two strategies agree on a subarray they did find.
public sealed partial class ShortestSubarrayWithSumAtLeastKBenchmarksTests
{
    private const int SmallestLength = 400;

    // The problem's own "no subarray reaches K" answer, and the value both arms must return
    // for this workload: K is unreachable, so neither strategy may report a subarray length.
    private const int ExpectedUnreachableTarget = -1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameValues() =>
        Assert.Equal(BuildHarness().BruteForcePrefixScan(), BuildHarness().BruteForcePrefixScan());

    [Fact]
    public void BruteForcePrefixScan_UnreachableTargetSum_AgreesWithMonotonicDequePrefixScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicDequePrefixScan(), harness.BruteForcePrefixScan());
        Assert.Equal(ExpectedUnreachableTarget, harness.MonotonicDequePrefixScan());
    }

    [Fact]
    public void MonotonicDequePrefixScan_UnreachableTargetSum_AgreesWithBruteForcePrefixScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForcePrefixScan(), harness.MonotonicDequePrefixScan());
        Assert.Equal(ExpectedUnreachableTarget, harness.BruteForcePrefixScan());
    }

    private static ShortestSubarrayWithSumAtLeastKBenchmarks BuildHarness()
    {
        var harness = new ShortestSubarrayWithSumAtLeastKBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
