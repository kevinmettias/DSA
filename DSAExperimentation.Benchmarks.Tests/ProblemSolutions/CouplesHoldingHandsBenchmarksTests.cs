using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CouplesHoldingHandsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - simulating the greedy seat swap against counting the row's
// disjoint-set components - so a harness whose arms disagree is timing two different problems, not
// two ways of answering one. Setup shuffles the row from one fixed seed, so the same CoupleCount must
// rebuild the same row; otherwise two published numbers were never comparable in the first place.
//
// The row is private and the swap count is the only thing either arm reports, so the documented shape
// is asserted through that: the answer is a minimum over a row of CoupleCount couples, and placing
// every couple correctly never needs more than CoupleCount - 1 swaps.
public sealed partial class CouplesHoldingHandsBenchmarksTests
{
    private const int SmallestCoupleCount = 200;

    private const int LargestMinimumSwaps = SmallestCoupleCount - 1;

    [Fact]
    public void Setup_SameCoupleCount_RebuildsTheSameRow()
    {
        Assert.InRange(BuildHarness().GreedySwapSimulation(), 0, LargestMinimumSwaps);
        Assert.Equal(BuildHarness().GreedySwapSimulation(), BuildHarness().GreedySwapSimulation());
    }

    [Fact]
    public void GreedySwapSimulation_ShuffledRow_AgreesWithDisjointSetComponentCounting()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetComponentCounting(), harness.GreedySwapSimulation());
    }

    [Fact]
    public void DisjointSetComponentCounting_ShuffledRow_AgreesWithGreedySwapSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GreedySwapSimulation(), harness.DisjointSetComponentCounting());
    }

    private static CouplesHoldingHandsBenchmarks BuildHarness()
    {
        var harness = new CouplesHoldingHandsBenchmarks { CoupleCount = SmallestCoupleCount };
        harness.Setup();

        return harness;
    }
}
