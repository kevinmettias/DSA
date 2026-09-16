using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CherryPickupBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the exponential (row1, col1, col2) recursion against the same
// recurrence remembered in the Memoizer - so a harness whose arms disagree is timing two different
// problems. Both arms answer with a single int, the most cherries a round trip can bank, so
// agreement between them says the two walks settled on the same total for the same grid.
public sealed partial class CherryPickupBenchmarksTests
{
    private const int SmallestSize = 4;

    // The round trip is two walkers crossing the all-cherry Size x Size grid together, right and down
    // only, with a cell they share counted once. Each walker covers 2 * Size - 1 = 7 cells, so the
    // two of them make 14 cell visits and must share at least the two ends of the walk, leaving at
    // most 12 distinct cells - and taking the top row then the right column for one walker and the
    // left column then the bottom row for the other shares exactly those two ends, covering 12 of the
    // 16 cells. With every cell worth one cherry, 12 is both achievable and the ceiling, so a rebuilt
    // grid has to bank it.
    private const int ExpectedCherries = 12;

    [Fact]
    public void Setup_SameSize_RebuildsTheAllCherryGrid()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(ExpectedCherries, first.UnmemoizedRecursion());
        Assert.Equal(ExpectedCherries, second.MemoizedRecursion());
    }

    [Fact]
    public void UnmemoizedRecursion_AllCherryFourByFourGrid_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecursion_AllCherryFourByFourGrid_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion());
    }

    private static CherryPickupBenchmarks BuildHarness()
    {
        var harness = new CherryPickupBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
