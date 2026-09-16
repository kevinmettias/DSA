using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitBenchmarks
// (ARCHITECTURE 17.9): its two arms are competing strategies for the same question - restarting a
// scan at every start index against the two monotonic deques that keep the window's min and max -
// so a harness whose arms disagree is timing two different problems. Both arms return the longest
// subarray length, a scalar compared directly. Setup draws values from a fixed seed over a narrow
// range relative to Limit, which is what keeps windows long; the same Length must rebuild them.
public sealed partial class LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().DoubleMonotonicDeque(),
            BuildHarness().DoubleMonotonicDeque());

    [Fact]
    public void BruteForceAllStartingPoints_SmallestLength_AgreesWithDoubleMonotonicDeque()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DoubleMonotonicDeque(), harness.BruteForceAllStartingPoints());
    }

    [Fact]
    public void DoubleMonotonicDeque_SmallestLength_AgreesWithBruteForceAllStartingPoints()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceAllStartingPoints(), harness.DoubleMonotonicDeque());
    }

    private static LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitBenchmarks BuildHarness()
    {
        var harness = new LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitBenchmarks
        {
            Length = SmallestLength,
        };
        harness.Setup();

        return harness;
    }
}
