using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GuessNumberHigherOrLowerIIBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing at the smallest size they share, so this asserts only the
// answer LeetCode 375's own statement fixes independently of either arm: 16 is the amount needed to
// guarantee a win over ten numbers.
public sealed partial class GuessNumberHigherOrLowerIIBenchmarksTests
{
    private const int SmallestHighestNumber = 10;

    // LeetCode 375's own example: ten numbers cost 16 to guarantee a win.
    private const int ExpectedGuaranteedCost = 16;

    [Fact]
    public void UnmemoizedRecursion_TenNumbers_CostsSixteenToGuaranteeAWin() =>
        Assert.Equal(ExpectedGuaranteedCost, BuildHarness().UnmemoizedRecursion(SmallestHighestNumber));

    private static GuessNumberHigherOrLowerIIBenchmarks BuildHarness() =>
        new();
}
