using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Check if There Is a Valid Parentheses String Path (LC 2267): un-memoized
// recursion re-explores every shared (Row, Col, Balance) state from scratch - up to
// C(2n-2, n-1) leaf calls on an n x n grid - vs. this repo's own Memoizer
// collapsing it to the polynomial distinct-state count, the same un-memoized-vs-
// Memoizer shape CherryPickupBenchmarks already proves out for a grid DP. The grid
// is all '(' so nothing short-circuits the naive baseline's full right/down
// branching early (balance only ever grows, never goes negative); Size is kept
// modest for exactly that reason.
[MemoryDiagnoser]
public class CheckIfThereIsAValidParenthesesStringPathBenchmarks
{
    [Params(8, 12)]
    public int Size;

    private char[,] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        _grid = new char[Size, Size];

        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                _grid[row, col] = '(';
            }
        }
    }

    [Benchmark(Baseline = true)]
    public bool UnmemoizedRecursion() => HasValidPathFrom(0, 0, 0);

    private bool HasValidPathFrom(int row, int col, int balance)
    {
        var (isTerminal, terminalValue, newBalance) = EvaluateState(row, col, balance);

        if (isTerminal)
        {
            return terminalValue;
        }

        var canGoDown = row + 1 < Size && HasValidPathFrom(row + 1, col, newBalance);
        var canGoRight = col + 1 < Size && HasValidPathFrom(row, col + 1, newBalance);
        return canGoDown || canGoRight;
    }

    [Benchmark]
    public bool MemoizedRecursion()
    {
        return Memoizer.Memoize<(int Row, int Col, int Balance), bool>((0, 0, 0), HasValidPathFromMemoized);

        bool HasValidPathFromMemoized(
            (int Row, int Col, int Balance) state, Func<(int Row, int Col, int Balance), bool> hasValidPath)
        {
            var (row, col, balance) = state;
            var (isTerminal, terminalValue, newBalance) = EvaluateState(row, col, balance);

            if (isTerminal)
            {
                return terminalValue;
            }

            var canGoDown = row + 1 < Size && hasValidPath((row + 1, col, newBalance));
            var canGoRight = col + 1 < Size && hasValidPath((row, col + 1, newBalance));
            return canGoDown || canGoRight;
        }
    }

    // Shared shape between the un-memoized and memoized walks: given a state, decide
    // whether it's terminal (balance went negative, or the destination was reached),
    // and if not, the balance after entering it. Neither branch here recurses - only
    // the caller knows how.
    private (bool IsTerminal, bool TerminalValue, int NewBalance) EvaluateState(int row, int col, int balance)
    {
        var newBalance = balance + (_grid[row, col] == '(' ? 1 : -1);

        if (newBalance < 0)
        {
            return (true, false, newBalance);
        }

        if (row == Size - 1 && col == Size - 1)
        {
            return (true, newBalance == 0, newBalance);
        }

        return (false, false, newBalance);
    }
}
