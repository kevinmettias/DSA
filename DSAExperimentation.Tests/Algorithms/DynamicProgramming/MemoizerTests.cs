using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Tests.Algorithms.DynamicProgramming.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.DynamicProgramming;

public sealed partial class MemoizerTests
{
    private const int GridRows = 3;
    private const int GridCols = 3;

    [Fact]
    public void Memoize_Fibonacci_ReturnsCorrectValue()
    {
        var result = Memoizer.Memoize<int, long>(10, new WaysFromPreviousTwoSteps());

        Assert.Equal(55, result);
    }

    [Fact]
    public void Memoize_Fibonacci_CallsRecurrenceOncePerDistinctState()
    {
        var recurrence = new CountedRecurrence<int, long>(new WaysFromPreviousTwoSteps());

        // Naive unmemoized fib(30) makes ~2.7M calls; memoized makes exactly one
        // per distinct state 0..30.
        Memoizer.Memoize<int, long>(30, recurrence);

        Assert.Equal(31, recurrence.Calls);
    }

    [Fact]
    public void Memoize_TupleState_CountsUniquePathsInGrid()
    {
        var result = Memoizer.Memoize<(int, int), long>((0, 0), new DownAndRightPaths(GridRows, GridCols));

        Assert.Equal(6, result);
    }

    [Fact]
    public void Memoize_TupleState_CallsRecurrenceOncePerDistinctCell()
    {
        var recurrence = new CountedRecurrence<(int, int), long>(new DownAndRightPaths(GridRows, GridCols));

        Memoizer.Memoize<(int, int), long>((0, 0), recurrence);

        // (0,0),(1,0),(0,1),(2,0),(1,1),(0,2),(2,1),(1,2) - every cell a down/right
        // walk actually queries before hitting an edge. (2,2) itself is never
        // queried: every path already returned at the edge one step before it.
        Assert.Equal(8, recurrence.Calls);
    }

    [Fact]
    public void Memoize_WithCustomComparer_TreatsComparerEqualStatesAsSameSubproblem()
    {
        var recurrence = new CountedRecurrence<string, int>(
            new TwoSpellingsOfOneState("start", "a", "A"));

        var result = Memoizer.Memoize<string, int>("start", recurrence, StringComparer.OrdinalIgnoreCase);

        Assert.Equal(2, result);
        Assert.Equal(2, recurrence.Calls);
    }

    [Fact]
    public void Memoize_StartStateIsBaseCase_ReturnsWithoutRecursing()
    {
        var recurrence = new CountedRecurrence<int, int>(new TheStateIsItsOwnAnswer());

        var result = Memoizer.Memoize<int, int>(0, recurrence);

        Assert.Equal(0, result);
        Assert.Equal(1, recurrence.Calls);
    }
}
