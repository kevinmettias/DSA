using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CouplesHoldingHandsBenchmarks (ARCHITECTURE 17.9): the generic arm check
// compares the arms only with each other, so what stays here is a bound derived without either arm.
// The row is private and the swap count is the only thing either arm reports, so the documented shape
// is asserted through that: the answer is a minimum over a row of CoupleCount couples, and placing
// every couple correctly never needs more than CoupleCount - 1 swaps.
public sealed partial class CouplesHoldingHandsBenchmarksTests
{
    private const int SmallestCoupleCount = 10;

    private const int LargestMinimumSwaps = SmallestCoupleCount - 1;

    [Fact]
    public void GreedySwapSimulation_ShuffledRow_NeedsFewerSwapsThanCouples() =>
        Assert.InRange(BuildHarness().GreedySwapSimulation(), 0, LargestMinimumSwaps);

    private static CouplesHoldingHandsBenchmarks BuildHarness()
    {
        var harness = new CouplesHoldingHandsBenchmarks { CoupleCount = SmallestCoupleCount };
        harness.Setup();

        return harness;
    }
}
