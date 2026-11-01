using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StoneGameBenchmarks (ARCHITECTURE 17.9): whether Alice wins LC 877 is
// known without running either arm, so the baseline is asserted against it - the generic arm
// check only holds the memoized arm to the baseline's answer, which two arms sharing one wrong
// verdict would pass.
public sealed partial class StoneGameBenchmarksTests
{
    private const int SmallestPileCount = 22;

    // LC 877 fixes an even pile count, and with an even count the first player can always take
    // every pile of one parity, whichever parity sums higher - so Alice wins every workload
    // this fixture can generate, whichever arm is asked.
    private const bool ExpectedAliceWins = true;

    [Fact]
    public void CanAliceWinByUnmemoizedRecursion_EvenPileCount_AliceWins() =>
        Assert.Equal(ExpectedAliceWins, BuildHarness().CanAliceWinByUnmemoizedRecursion());

    private static StoneGameBenchmarks BuildHarness()
    {
        var harness = new StoneGameBenchmarks { PileCount = SmallestPileCount };
        harness.Setup();

        return harness;
    }
}
