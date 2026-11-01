using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfThereIsAValidParenthesesStringPathBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt grid, so this pins the
// baseline to the verdict the grid's shape decides, derived independently of either arm.
public sealed partial class CheckIfThereIsAValidParenthesesStringPathBenchmarksTests
{
    private const int SmallestSize = 8;

    // Setup fills the whole grid with '(', a path from (0, 0) to (Size - 1, Size - 1) is 2 * Size - 1
    // characters long, and every one of them is an opening parenthesis: such a path ends on a balance
    // of 2 * Size - 1, never the 0 a valid path requires, so no path in the grid can be valid. That
    // is the shape the harness wants - nothing short-circuits the un-memoized arm's branching early.
    private const bool ExpectedVerdict = false;

    [Fact]
    public void HasValidPathByUnmemoizedRecursion_AllOpenGrid_ReturnsFalse() =>
        Assert.Equal(ExpectedVerdict, BuildHarness().HasValidPathByUnmemoizedRecursion(SmallestSize));

    private static CheckIfThereIsAValidParenthesesStringPathBenchmarks BuildHarness()
    {
        var harness = new CheckIfThereIsAValidParenthesesStringPathBenchmarks();
        harness.Setup();

        return harness;
    }
}
