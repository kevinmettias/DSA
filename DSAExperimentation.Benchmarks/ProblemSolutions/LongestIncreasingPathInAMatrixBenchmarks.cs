using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestIncreasingPathInAMatrix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestIncreasingPathInAMatrixSolution's, the same
// methods LongestIncreasingPathInAMatrixTests proves correct, run over a strictly
// row-major-increasing Size x Size matrix (so every cell's only increasing neighbors
// are right/down, the classic Unique-Paths-shaped DAG with heavy path overlap).
// NaiveRecursion re-explores every shared sub-path from scratch per candidate start
// cell - central-Delannoy-number growth, kept modest for exactly that reason.
// MemoizedRecurrence caches sub-paths revisited within one start's search, collapsing
// that call from exponential to polynomial.
[MemoryDiagnoser]
public class LongestIncreasingPathInAMatrixBenchmarks
{
    private int[,] _matrix = new int[0, 0];

    [Params(6, 9)]
    public int Size { get; set; }

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
    public int NaiveRecursion() => LongestIncreasingPathInAMatrixSolution.LongestPathByNaiveRecursion(_matrix);

    [Benchmark]
    public int MemoizedRecurrence() =>
        LongestIncreasingPathInAMatrixSolution.LongestPathByMemoizedRecurrence(_matrix);
}
