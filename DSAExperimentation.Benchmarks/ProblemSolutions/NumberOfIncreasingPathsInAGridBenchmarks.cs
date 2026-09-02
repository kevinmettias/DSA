using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Increasing Paths in a Grid (LC 2328): the same row-major strictly-
// increasing Size x Size matrix LongestIncreasingPathInAMatrixBenchmarks uses (every
// cell's only increasing neighbors are right/down, the classic Unique-Paths-shaped
// DAG with heavy path overlap). NaiveRecursive re-walks every shared sub-path from
// scratch per candidate start cell - here the call count itself, not just the
// returned value, grows with the number of increasing paths (central-Delannoy-
// number territory). MemoizedRecurrence dogfoods this repo's own Memoizer per start
// cell, collapsing shared sub-paths within one start's search from exponential to
// polynomial.
[MemoryDiagnoser]
public class NumberOfIncreasingPathsInAGridBenchmarks
{
    private const long Mod = 1_000_000_007L;
    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    [Params(6, 9)]
    public int Size;

    private int[,] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        _matrix = new int[Size, Size];

        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                _matrix[row, col] = row * Size + col;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public long NaiveRecursive()
    {
        var total = 0L;

        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                total = (total + CountFromNaive(row, col)) % Mod;
            }
        }

        return total;
    }

    private long CountFromNaive(int row, int col)
    {
        var count = 1L;

        foreach (var (rowOffset, colOffset) in Directions)
        {
            var nextRow = row + rowOffset;
            var nextCol = col + colOffset;

            if (nextRow >= 0 && nextRow < Size && nextCol >= 0 && nextCol < Size
                && _matrix[nextRow, nextCol] > _matrix[row, col])
            {
                count += CountFromNaive(nextRow, nextCol);
            }
        }

        return count % Mod;
    }

    [Benchmark]
    public long MemoizedRecurrence()
    {
        var total = 0L;

        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                total = (total + Memoizer.Memoize<(int Row, int Col), long>((row, col), CountFromMemoized)) % Mod;
            }
        }

        return total;
    }

    private long CountFromMemoized((int Row, int Col) state, Func<(int Row, int Col), long> countFrom)
    {
        var count = 1L;

        foreach (var (rowOffset, colOffset) in Directions)
        {
            var nextRow = state.Row + rowOffset;
            var nextCol = state.Col + colOffset;

            if (nextRow >= 0 && nextRow < Size && nextCol >= 0 && nextCol < Size
                && _matrix[nextRow, nextCol] > _matrix[state.Row, state.Col])
            {
                count += countFrom((nextRow, nextCol));
            }
        }

        return count % Mod;
    }
}
