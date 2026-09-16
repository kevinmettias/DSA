using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountNoZeroPairsThatSumToNBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - scanning every split of the target against the
// memoized digit DP - so a harness whose arms disagree is timing two different problems. This
// benchmark has no [GlobalSetup]: TargetSum is the whole workload, so there is nothing to rebuild.
public sealed partial class CountNoZeroPairsThatSumToNBenchmarksTests
{
    private const long SmallestTargetSum = 100_000;

    [Fact]
    public void BruteForceSplit_SmallestTargetSum_AgreesWithMemoizedDigitDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedDigitDp(), harness.BruteForceSplit());
    }

    [Fact]
    public void MemoizedDigitDp_SmallestTargetSum_AgreesWithBruteForceSplit()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceSplit(), harness.MemoizedDigitDp());
    }

    private static CountNoZeroPairsThatSumToNBenchmarks BuildHarness() =>
        new() { TargetSum = SmallestTargetSum };
}
