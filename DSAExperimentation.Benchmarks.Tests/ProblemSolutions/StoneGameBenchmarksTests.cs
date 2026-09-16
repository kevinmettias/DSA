using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StoneGameBenchmarks (ARCHITECTURE 17.9): both arms answer the same
// question - whether Alice wins LC 877 - one by plain minimax over (left, right) bounds, one by
// memoizing that pair, so a harness whose arms disagree is timing two different problems.
// Setup's piles are seeded, so the same pile count must rebuild the same workload.
public sealed partial class StoneGameBenchmarksTests
{
    private const int SmallestPileCount = 22;

    // LC 877 fixes an even pile count, and with an even count the first player can always take
    // every pile of one parity, whichever parity sums higher - so Alice wins every workload
    // this fixture can generate, whichever arm is asked.
    private const bool ExpectedAliceWins = true;

    [Fact]
    public void Setup_SamePileCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().CanAliceWinByMemoizedRecursion()),
            AnswerText.Of(BuildHarness().CanAliceWinByMemoizedRecursion()));

    [Fact]
    public void CanAliceWinByUnmemoizedRecursion_AgreesWithCanAliceWinByMemoizedRecursion()
    {
        var harness = BuildHarness();
        var unmemoized = harness.CanAliceWinByUnmemoizedRecursion();

        Assert.Equal(unmemoized, harness.CanAliceWinByMemoizedRecursion());
        Assert.Equal(ExpectedAliceWins, unmemoized);
    }

    [Fact]
    public void CanAliceWinByMemoizedRecursion_AgreesWithCanAliceWinByUnmemoizedRecursion()
    {
        var harness = BuildHarness();
        var memoized = harness.CanAliceWinByMemoizedRecursion();

        Assert.Equal(memoized, harness.CanAliceWinByUnmemoizedRecursion());
        Assert.Equal(ExpectedAliceWins, memoized);
    }

    private static StoneGameBenchmarks BuildHarness()
    {
        var harness = new StoneGameBenchmarks { PileCount = SmallestPileCount };
        harness.Setup();

        return harness;
    }
}
