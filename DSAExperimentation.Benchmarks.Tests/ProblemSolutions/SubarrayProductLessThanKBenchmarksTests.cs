using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SubarrayProductLessThanKBenchmarks (ARCHITECTURE 17.9): both arms answer
// the same question - how many LC 713 subarrays have a product under K - one by the plain
// double loop, one by a log-prefix lower bound, so a harness whose arms disagree is timing two
// different problems. Setup repeats a single value, so the same length must rebuild the same
// workload.
//
// The fixture's values are all 1 and K is 2, so every subarray qualifies: the count lands on a
// value the fixture's own structure fixes, not merely on whatever both arms happen to say.
public sealed partial class SubarrayProductLessThanKBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every one of the SmallestLength * (SmallestLength + 1) / 2 contiguous subarrays has
    // product 1, which is below K, so all of them are counted.
    private const int ExpectedSubarrayCount = SmallestLength * (SmallestLength + 1) / 2;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_AgreesWithLogPrefixLowerBound()
    {
        var harness = BuildHarness();
        var bruteForce = harness.BruteForce();

        Assert.Equal(bruteForce, harness.LogPrefixLowerBound());
        Assert.Equal(ExpectedSubarrayCount, bruteForce);
    }

    [Fact]
    public void LogPrefixLowerBound_AgreesWithBruteForce()
    {
        var harness = BuildHarness();
        var logPrefix = harness.LogPrefixLowerBound();

        Assert.Equal(logPrefix, harness.BruteForce());
        Assert.Equal(ExpectedSubarrayCount, logPrefix);
    }

    private static SubarrayProductLessThanKBenchmarks BuildHarness()
    {
        var harness = new SubarrayProductLessThanKBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
