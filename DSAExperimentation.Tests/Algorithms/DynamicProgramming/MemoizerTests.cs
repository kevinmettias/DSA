using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.Algorithms.DynamicProgramming;

public sealed partial class MemoizerTests
{
    private const int GridRows = 3;
    private const int GridCols = 3;

    [Fact]
    public void Memoize_Fibonacci_ReturnsCorrectValue()
    {
        long Recurrence(int n, Func<int, long> fib) => n <= 1 ? n : fib(n - 1) + fib(n - 2);

        var result = Memoizer.Memoize<int, long>(10, Recurrence);

        Assert.Equal(55, result);
    }

    [Fact]
    public void Memoize_Fibonacci_CallsRecurrenceOncePerDistinctState()
    {
        var calls = 0;

        long Recurrence(int n, Func<int, long> fib)
        {
            calls++;
            return n <= 1 ? n : fib(n - 1) + fib(n - 2);
        }

        // Naive unmemoized fib(30) makes ~2.7M calls; memoized makes exactly one
        // per distinct state 0..30.
        Memoizer.Memoize<int, long>(30, Recurrence);

        Assert.Equal(31, calls);
    }

    [Fact]
    public void Memoize_TupleState_CountsUniquePathsInGrid()
    {
        long Recurrence((int Row, int Col) state, Func<(int, int), long> countFrom)
        {
            var (row, col) = state;
            return row == GridRows - 1 || col == GridCols - 1
                ? 1
                : countFrom((row + 1, col)) + countFrom((row, col + 1));
        }

        var result = Memoizer.Memoize<(int, int), long>((0, 0), Recurrence);

        Assert.Equal(6, result);
    }

    [Fact]
    public void Memoize_TupleState_CallsRecurrenceOncePerDistinctCell()
    {
        var calls = 0;

        long Recurrence((int Row, int Col) state, Func<(int, int), long> countFrom)
        {
            calls++;
            var (row, col) = state;
            return row == GridRows - 1 || col == GridCols - 1
                ? 1
                : countFrom((row + 1, col)) + countFrom((row, col + 1));
        }

        Memoizer.Memoize<(int, int), long>((0, 0), Recurrence);

        // (0,0),(1,0),(0,1),(2,0),(1,1),(0,2),(2,1),(1,2) - every cell a down/right
        // walk actually queries before hitting an edge. (2,2) itself is never
        // queried: every path already returned at the edge one step before it.
        Assert.Equal(8, calls);
    }

    [Fact]
    public void Memoize_WithCustomComparer_TreatsComparerEqualStatesAsSameSubproblem()
    {
        var calls = 0;

        int Recurrence(string state, Func<string, int> lookup)
        {
            calls++;
            return state == "start" ? lookup("a") + lookup("A") : 1;
        }

        var result = Memoizer.Memoize<string, int>("start", Recurrence, StringComparer.OrdinalIgnoreCase);

        Assert.Equal(2, result);
        Assert.Equal(2, calls);
    }

    [Fact]
    public void Memoize_StartStateIsBaseCase_ReturnsWithoutRecursing()
    {
        var calls = 0;

        int Recurrence(int n, Func<int, int> _)
        {
            calls++;
            return n;
        }

        var result = Memoizer.Memoize<int, int>(0, Recurrence);

        Assert.Equal(0, result);
        Assert.Equal(1, calls);
    }
}
