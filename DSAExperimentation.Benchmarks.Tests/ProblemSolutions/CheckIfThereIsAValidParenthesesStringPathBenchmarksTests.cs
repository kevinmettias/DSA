using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfThereIsAValidParenthesesStringPathBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - the unmemoized right/down branching
// against the same recurrence remembered per (row, column, balance) - so a harness whose arms
// disagree is timing two different problems. Both arms answer with a bare bool, so agreement between
// them says the two searches reached the same verdict on the same grid.
public sealed partial class CheckIfThereIsAValidParenthesesStringPathBenchmarksTests
{
    private const int SmallestSize = 8;

    // Setup fills the whole grid with '(', a path from (0, 0) to (Size - 1, Size - 1) is 2 * Size - 1
    // characters long, and every one of them is an opening parenthesis: such a path ends on a balance
    // of 2 * Size - 1, never the 0 a valid path requires, and the balance never dips below zero on
    // the way there, so no path in the rebuilt grid can be valid. That is the shape the harness wants
    // - nothing short-circuits the un-memoized arm's branching early - and it is what makes the
    // rebuilt grid's verdict decisive.
    private const bool ExpectedVerdict = false;

    [Fact]
    public void Setup_SameSize_RebuildsTheAllOpenGrid()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(ExpectedVerdict, first.HasValidPathByUnmemoizedRecursion());
        Assert.Equal(ExpectedVerdict, second.HasValidPathByMemoizedRecursion());
    }

    [Fact]
    public void HasValidPathByUnmemoizedRecursion_AllOpenGrid_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasValidPathByMemoizedRecursion(), harness.HasValidPathByUnmemoizedRecursion());
    }

    [Fact]
    public void HasValidPathByMemoizedRecursion_AllOpenGrid_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasValidPathByUnmemoizedRecursion(), harness.HasValidPathByMemoizedRecursion());
    }

    private static CheckIfThereIsAValidParenthesesStringPathBenchmarks BuildHarness()
    {
        var harness = new CheckIfThereIsAValidParenthesesStringPathBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
