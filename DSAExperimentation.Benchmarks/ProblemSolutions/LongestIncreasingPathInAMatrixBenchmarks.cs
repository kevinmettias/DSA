using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Increasing Path in a Matrix (LC 329): both benchmarks run the exact same
// four-directional recurrence over a strictly row-major-increasing Size x Size
// matrix (so every cell's only increasing neighbors are right/down, the classic
// Unique-Paths-shaped DAG with heavy path overlap). NaiveRecursive re-explores every
// shared sub-path from scratch per candidate start cell - central-Delannoy-number
// growth, kept modest for exactly that reason. MemoizedRecurrence dogfoods this
// repo's own Memoizer per start cell; sub-paths revisited within one start's search
// are cached, collapsing that call from exponential to polynomial.
[MemoryDiagnoser]
public class LongestIncreasingPathInAMatrixBenchmarks
{
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
    public int NaiveRecursive()
    {
        var best = 0;

        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                best = Math.Max(best, LengthFrom(row, col));
            }
        }

        return best;

        int LengthFrom(int row, int col)
        {
            var longest = 1;

            foreach (var (rowOffset, colOffset) in Directions)
            {
                var nextRow = row + rowOffset;
                var nextCol = col + colOffset;

                if (nextRow >= 0 && nextRow < Size && nextCol >= 0 && nextCol < Size
                    && _matrix[nextRow, nextCol] > _matrix[row, col])
                {
                    longest = Math.Max(longest, 1 + LengthFrom(nextRow, nextCol));
                }
            }

            return longest;
        }
    }

    [Benchmark]
    public int MemoizedRecurrence()
    {
        var best = 0;

        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                best = Math.Max(best, Memoizer.Memoize<(int Row, int Col), int>((row, col), LengthFrom));
            }
        }

        return best;

        int LengthFrom((int Row, int Col) state, Func<(int Row, int Col), int> lengthFrom)
        {
            var longest = 1;

            foreach (var (rowOffset, colOffset) in Directions)
            {
                var nextRow = state.Row + rowOffset;
                var nextCol = state.Col + colOffset;

                if (nextRow >= 0 && nextRow < Size && nextCol >= 0 && nextCol < Size
                    && _matrix[nextRow, nextCol] > _matrix[state.Row, state.Col])
                {
                    longest = Math.Max(longest, 1 + lengthFrom((nextRow, nextCol)));
                }
            }

            return longest;
        }
    }
}
