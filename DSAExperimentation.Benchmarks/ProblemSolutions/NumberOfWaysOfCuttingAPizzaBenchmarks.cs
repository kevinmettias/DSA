using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Ways of Cutting a Pizza (LC 1444): plain un-memoized (Row, Col,
// RemainingCuts) recursion over every possible horizontal/vertical cut - exponential,
// since the same (Row, Col, RemainingCuts) state recurs across many different earlier
// cut sequences that land on it - vs. this repo's own Memoizer<TState,TResult> caching
// that exact triple (the same CherryPickupBenchmarks/BurstBalloonsBenchmarks
// un-memoized-vs-Memoizer shape). Both share the same O(1) suffix-sum apple lookup;
// only the caching differs. The grid is all-apples so nothing short-circuits the naive
// baseline's full branching early; Size/K are kept modest for exactly that reason.
[MemoryDiagnoser]
public class NumberOfWaysOfCuttingAPizzaBenchmarks
{
    private const int Modulus = 1_000_000_007;
    private const int Cuts = 4;

    [Params(6, 8)]
    public int Size;

    private int[,] _apples = null!;

    [GlobalSetup]
    public void Setup()
    {
        _apples = new int[Size + 1, Size + 1];

        for (var row = Size - 1; row >= 0; row--)
        {
            for (var col = Size - 1; col >= 0; col--)
            {
                _apples[row, col] = 1 + _apples[row + 1, col] + _apples[row, col + 1] - _apples[row + 1, col + 1];
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => WaysFrom(0, 0, Cuts);

    private int WaysFrom(int row, int col, int remainingCuts)
    {
        if (_apples[row, col] == 0)
        {
            return 0;
        }

        if (remainingCuts == 0)
        {
            return 1;
        }

        var total = HorizontalCutWays(row, col, remainingCuts);
        return (total + VerticalCutWays(row, col, remainingCuts)) % Modulus;
    }

    private int HorizontalCutWays(int row, int col, int remainingCuts)
    {
        var total = 0;

        for (var nextRow = row + 1; nextRow < Size; nextRow++)
        {
            if (_apples[row, col] - _apples[nextRow, col] > 0)
            {
                total = (total + WaysFrom(nextRow, col, remainingCuts - 1)) % Modulus;
            }
        }

        return total;
    }

    private int VerticalCutWays(int row, int col, int remainingCuts)
    {
        var total = 0;

        for (var nextCol = col + 1; nextCol < Size; nextCol++)
        {
            if (_apples[row, col] - _apples[row, nextCol] > 0)
            {
                total = (total + WaysFrom(row, nextCol, remainingCuts - 1)) % Modulus;
            }
        }

        return total;
    }

    [Benchmark]
    public int MemoizedRecursion()
        => Memoizer.Memoize<(int Row, int Col, int RemainingCuts), int>((0, 0, Cuts), WaysFromMemoized);

    private int WaysFromMemoized(
        (int Row, int Col, int RemainingCuts) state,
        Func<(int Row, int Col, int RemainingCuts), int> waysFrom)
    {
        var (row, col, remainingCuts) = state;

        if (_apples[row, col] == 0)
        {
            return 0;
        }

        if (remainingCuts == 0)
        {
            return 1;
        }

        var total = HorizontalCutWaysMemoized(row, col, remainingCuts, waysFrom);
        return (total + VerticalCutWaysMemoized(row, col, remainingCuts, waysFrom)) % Modulus;
    }

    private int HorizontalCutWaysMemoized(
        int row, int col, int remainingCuts, Func<(int Row, int Col, int RemainingCuts), int> waysFrom)
    {
        var total = 0;

        for (var nextRow = row + 1; nextRow < Size; nextRow++)
        {
            if (_apples[row, col] - _apples[nextRow, col] > 0)
            {
                total = (total + waysFrom((nextRow, col, remainingCuts - 1))) % Modulus;
            }
        }

        return total;
    }

    private int VerticalCutWaysMemoized(
        int row, int col, int remainingCuts, Func<(int Row, int Col, int RemainingCuts), int> waysFrom)
    {
        var total = 0;

        for (var nextCol = col + 1; nextCol < Size; nextCol++)
        {
            if (_apples[row, col] - _apples[row, nextCol] > 0)
            {
                total = (total + waysFrom((row, nextCol, remainingCuts - 1))) % Modulus;
            }
        }

        return total;
    }
}
