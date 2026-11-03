using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SubarrayWithElementsGreaterThanVaryingThresholdBenchmarks
// (ARCHITECTURE 17.9): the fixture's threshold is deliberately unreachable, so both arms are
// forced through their full scan and must report LC 2334's no-valid-subarray answer. The generic
// arm check's agreement alone would also hold for two arms that both returned something else,
// which is why the expectation is asserted of the baseline.
public sealed partial class SubarrayWithElementsGreaterThanVaryingThresholdBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void WindowMinimumScan_UnreachableThreshold_ReportsNoValidSubarray() =>
        Assert.Equal(LeetCodeAnswer.None, BuildHarness().WindowMinimumScan());

    private static SubarrayWithElementsGreaterThanVaryingThresholdBenchmarks BuildHarness()
    {
        var harness = new SubarrayWithElementsGreaterThanVaryingThresholdBenchmarks
        {
            Length = SmallestLength,
        };

        harness.Setup();

        return harness;
    }
}
