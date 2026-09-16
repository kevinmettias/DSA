using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GuessNumberHigherOrLowerIIBenchmarks (ARCHITECTURE 17.9): both arms are
// GuessNumberHigherOrLowerIISolution's - the un-memoized recursion against the memoized one - so a
// harness whose arms disagree is timing two different problems. The class carries no [GlobalSetup]
// at all: HighestNumber is the whole workload, with nothing to rebuild and no setup invariant to
// assert. Both arms answer with the guaranteed cost, and that cost is decisive at the smallest
// HighestNumber from LeetCode 375's own statement, which names 16 as the amount needed to guarantee
// a win over ten numbers. The parameters themselves are the workload, so two harnesses at the same
// HighestNumber must answer identically.
public sealed partial class GuessNumberHigherOrLowerIIBenchmarksTests
{
    private const int SmallestHighestNumber = 10;

    // LeetCode 375's own example: ten numbers cost 16 to guarantee a win.
    private const int ExpectedGuaranteedCost = 16;

    [Fact]
    public void UnmemoizedRecursion_TenNumbers_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedGuaranteedCost, harness.UnmemoizedRecursion());
        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecursion_TenNumbers_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedGuaranteedCost, harness.MemoizedRecursion());
        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion());
    }

    private static GuessNumberHigherOrLowerIIBenchmarks BuildHarness() =>
        new() { HighestNumber = SmallestHighestNumber };
}
