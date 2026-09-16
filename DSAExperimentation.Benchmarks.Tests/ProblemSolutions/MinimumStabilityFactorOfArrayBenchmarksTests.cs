using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumStabilityFactorOfArrayBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the smallest cap on the longest stable subarray that
// the allowed modifications can buy - so a harness whose arms disagree is timing two different
// problems. SegmentTreeGcd is handed the segment tree [GlobalSetup] already built over the same nums
// the scan arm is given, so the comparison also pins that the hoisted tree carries exactly those
// values and that its range gcd matches the rescan on every window the binary search asks about.
// Setup draws nums from one fixed seed, so the same Length must rebuild the same values.
public sealed partial class MinimumStabilityFactorOfArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceGcdScan(), BuildHarness().BruteForceGcdScan());

    [Fact]
    public void BruteForceGcdScan_SameValueRun_AgreesWithSegmentTreeGcd()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SegmentTreeGcd(), harness.BruteForceGcdScan());
    }

    [Fact]
    public void SegmentTreeGcd_SameValueRun_AgreesWithBruteForceGcdScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceGcdScan(), harness.SegmentTreeGcd());
    }

    private static MinimumStabilityFactorOfArrayBenchmarks BuildHarness()
    {
        var harness = new MinimumStabilityFactorOfArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
