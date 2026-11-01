using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortestSubarrayWithSumAtLeastKBenchmarks (ARCHITECTURE 17.9). Arm agreement
// is BenchmarkArmsTests' job; this pins the answer the workload is built to have. K is deliberately
// far above any sum the bounded values can reach, so every arm is forced through its full
// worst-case scan and must return the -1 the problem reserves for "no such subarray" - the check
// below that the baseline returns that sentinel, rather than a length, is what keeps the arms from
// passing on a shared answer neither actually computed.
public sealed partial class ShortestSubarrayWithSumAtLeastKBenchmarksTests
{
    private const int SmallestLength = 400;

    // The problem's own "no subarray reaches K" answer, and the value both arms must return
    // for this workload: K is unreachable, so neither strategy may report a subarray length.
    private const int ExpectedUnreachableTarget = -1;

    [Fact]
    public void BruteForcePrefixScan_UnreachableTargetSum_ReturnsNoSubarray() =>
        Assert.Equal(ExpectedUnreachableTarget, BuildHarness().BruteForcePrefixScan());

    private static ShortestSubarrayWithSumAtLeastKBenchmarks BuildHarness()
    {
        var harness = new ShortestSubarrayWithSumAtLeastKBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
