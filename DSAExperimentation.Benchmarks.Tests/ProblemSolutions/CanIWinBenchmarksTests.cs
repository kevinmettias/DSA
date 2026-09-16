using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CanIWinBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question - the raw game-tree recursion against the same recursion memoized on the
// used-number bitmask - so a harness whose arms disagree is timing two different problems. Both arms
// return only a bool, so an agreement between them is the honest assertion here and it is worth the
// least of any class in this batch: it says the two strategies reached the same verdict, not why.
// What keeps it from being vacuous is the workload Setup derives - desiredTotal is the exact sum of
// every choosable number, so the verdict can only be reached after the full search tree is explored
// (six moves deep at the smallest [Params]) rather than on the first pick. Setup derives that total
// from MaxChoosableInteger, so the same parameter must rebuild the same game.
public sealed partial class CanIWinBenchmarksTests
{
    private const int SmallestMaxChoosableInteger = 6;

    [Fact]
    public void Setup_SameMaxChoosableInteger_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().CanWinByBruteForceRecursion(), BuildHarness().CanWinByBruteForceRecursion());

    [Fact]
    public void CanWinByBruteForceRecursion_TotalEqualsTheSumOfEveryNumber_AgreesWithCanWinByMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanWinByMemoizedRecursion(), harness.CanWinByBruteForceRecursion());
    }

    [Fact]
    public void CanWinByMemoizedRecursion_TotalEqualsTheSumOfEveryNumber_AgreesWithCanWinByBruteForceRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanWinByBruteForceRecursion(), harness.CanWinByMemoizedRecursion());
    }

    private static CanIWinBenchmarks BuildHarness()
    {
        var harness = new CanIWinBenchmarks { MaxChoosableInteger = SmallestMaxChoosableInteger };
        harness.Setup();

        return harness;
    }
}
