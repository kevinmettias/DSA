using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfSubarraysThatMatchAPatternIIBenchmarks (ARCHITECTURE 17.9): the
// baseline's count against one derived from the workload's own layout rather than from either arm,
// which the generic check cannot supply - it holds the ZFunction arm to the baseline, and arms that
// drop the same windows agree just as well. nums is strictly increasing and pattern is all 1s, so
// every window matches: a matching window starts at every i with i + patternLength < numsLength.
public sealed partial class NumberOfSubarraysThatMatchAPatternIIBenchmarksTests
{
    private const int SmallestNumsLength = 1_000;

    // The fixture's pattern is this share of the nums length: an all-1s pattern that long matches at
    // every start i with i + patternLength < numsLength.
    private const int PatternShareDivisor = 2;

    [Fact]
    public void BruteForce_EveryWindowMatches_CountsEveryWindow()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedAllMatchingWindowCount(harness), harness.BruteForce());
    }

    private static NumberOfSubarraysThatMatchAPatternIIBenchmarks BuildHarness()
    {
        var harness = new NumberOfSubarraysThatMatchAPatternIIBenchmarks { NumsLength = SmallestNumsLength };
        harness.Setup();

        return harness;
    }

    // A strictly increasing nums makes every consecutive sign 1, and an all-1s pattern of half the
    // length therefore matches at every start i with i + patternLength < numsLength.
    private static int ExpectedAllMatchingWindowCount(NumberOfSubarraysThatMatchAPatternIIBenchmarks harness) =>
        harness.NumsLength - harness.NumsLength / PatternShareDivisor;
}
