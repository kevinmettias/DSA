using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CherryPickupBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already holds
// the arms to agreeing on a rebuilt grid, so this pins the baseline to the most cherries the
// all-cherry grid's shape allows, derived independently of either arm.
public sealed partial class CherryPickupBenchmarksTests
{
    private const int SmallestSize = 4;

    // The round trip is two walkers crossing the all-cherry Size x Size grid together, right and down
    // only, with a cell they share counted once. Each walker covers 2 * Size - 1 = 7 cells, so the
    // two of them make 14 cell visits and must share at least the two ends of the walk, leaving at
    // most 12 distinct cells - and taking the top row then the right column for one walker and the
    // left column then the bottom row for the other shares exactly those two ends, covering 12 of the
    // 16 cells. With every cell worth one cherry, 12 is both achievable and the ceiling.
    private const int ExpectedCherries = 12;

    [Fact]
    public void UnmemoizedRecursion_AllCherryFourByFourGrid_BanksTwelveCherries() =>
        Assert.Equal(ExpectedCherries, BuildHarness().UnmemoizedRecursion(SmallestSize));

    private static CherryPickupBenchmarks BuildHarness()
    {
        var harness = new CherryPickupBenchmarks();
        harness.Setup();

        return harness;
    }
}
