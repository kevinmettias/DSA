using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Cherry Pickup (LC 741): reframed as two simultaneous (0,0)->(n-1,n-1) walks sharing
// the same Row1+Col1 step count - the classic massively-overlapping-subproblem DP
// (UniquePaths-shaped, closed over four move combinations for the pair). Un-memoized
// recursion re-explores every shared (Row1, Col1, Col2) state from scratch - up to
// 4^(2n-2) calls - vs. this repo's own Memoizer collapsing it to the polynomial n^3
// distinct states (BurstBalloonsBenchmarks precedent for this same un-memoized-vs-
// Memoizer shape). The grid is all-cherries with no obstacles so nothing
// short-circuits the naive baseline's full branching early; Size is kept modest for
// exactly that reason.
[MemoryDiagnoser]
public class CherryPickupBenchmarks
{
    private const int Blocked = int.MinValue;

    [Params(4, 6)]
    public int Size;

    private int[,] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        _grid = new int[Size, Size];

        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                _grid[row, col] = 1;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion()
    {
        var cherries = CherriesFrom(0, 0, 0);
        return Math.Max(0, cherries);
    }

    private int CherriesFrom(int row1, int col1, int col2)
    {
        var (isTerminal, terminalValue, picked) = EvaluateCherryState(row1, col1, col2);

        if (isTerminal)
        {
            return terminalValue;
        }

        var moveDownAddCol2 = CherriesFrom(row1 + 1, col1, col2 + 1);
        var moveDownSameCol2 = CherriesFrom(row1 + 1, col1, col2);
        var moveRightAddCol2 = CherriesFrom(row1, col1 + 1, col2 + 1);
        var moveRightSameCol2 = CherriesFrom(row1, col1 + 1, col2);
        var bestNext = BestOfFour(moveDownAddCol2, moveDownSameCol2, moveRightAddCol2, moveRightSameCol2);

        return bestNext == Blocked ? Blocked : picked + bestNext;
    }

    [Benchmark]
    public int MemoizedRecursion()
    {
        var result = Memoizer.Memoize<(int Row1, int Col1, int Col2), int>((0, 0, 0), CherriesFromMemoized);
        return Math.Max(0, result);

        int CherriesFromMemoized(
            (int Row1, int Col1, int Col2) state,
            Func<(int Row1, int Col1, int Col2), int> cherriesFrom)
        {
            var (row1, col1, col2) = state;
            var (isTerminal, terminalValue, picked) = EvaluateCherryState(row1, col1, col2);

            if (isTerminal)
            {
                return terminalValue;
            }

            var moveDownAddCol2 = cherriesFrom((row1 + 1, col1, col2 + 1));
            var moveDownSameCol2 = cherriesFrom((row1 + 1, col1, col2));
            var moveRightAddCol2 = cherriesFrom((row1, col1 + 1, col2 + 1));
            var moveRightSameCol2 = cherriesFrom((row1, col1 + 1, col2));
            var bestNext = BestOfFour(moveDownAddCol2, moveDownSameCol2, moveRightAddCol2, moveRightSameCol2);

            return bestNext == Blocked ? Blocked : picked + bestNext;
        }
    }

    // Shared shape between the un-memoized and memoized walks: given a state, decide
    // whether it's a terminal (blocked/goal) value, and if not, the cherries picked up
    // by entering it. Neither branch here recurses - only the caller knows how.
    private (bool IsTerminal, int TerminalValue, int Picked) EvaluateCherryState(int row1, int col1, int col2)
    {
        var row2 = row1 + col1 - col2;

        if (row1 >= Size || col1 >= Size || row2 < 0 || row2 >= Size || col2 < 0 || col2 >= Size
            || _grid[row1, col1] == -1 || _grid[row2, col2] == -1)
        {
            return (true, Blocked, 0);
        }

        if (row1 == Size - 1 && col1 == Size - 1)
        {
            return (true, _grid[row1, col1], 0);
        }

        var picked = _grid[row1, col1] + (col1 == col2 ? 0 : _grid[row2, col2]);
        return (false, 0, picked);
    }

    private static int BestOfFour(int downAddCol2, int downSameCol2, int rightAddCol2, int rightSameCol2)
    {
        var downBest = Math.Max(downAddCol2, downSameCol2);
        var rightBest = Math.Max(rightAddCol2, rightSameCol2);
        return Math.Max(downBest, rightBest);
    }
}
