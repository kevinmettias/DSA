using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumStrictlyIncreasingCellsInAMatrix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumStrictlyIncreasingCellsInAMatrixSolution's, the
// same methods MaximumStrictlyIncreasingCellsInAMatrixTests proves correct.
// MemoizedRowColumnScan is the textbook DAG-DP baseline - each of the rows*cols cells
// rescans a whole row plus a whole column, O(rows*cols*(rows+cols)) - while
// SortedBatchDp MergeSorts every cell by value once and folds each equal-value batch
// into running rowBest/colBest arrays, O(rows*cols*log(rows*cols)) with no per-cell
// rescan, so the gap widens with the side length.
//
// Both arms take LeetCode's own jagged matrix, which is already the prepared input, so
// there is nothing for [GlobalSetup] to hoist beyond generating it (#17.4).
[MemoryDiagnoser]
public class MaximumStrictlyIncreasingCellsInAMatrixBenchmarks
{
    private const int RandomSeed = 2713; // LC problem number
    private const int ValueRange = 1_000;

    private int[][] _matrix = [];

    [Params(8, 20)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _matrix = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _matrix[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _matrix[r][c] = random.Next(1, ValueRange);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int MemoizedRowColumnScan() =>
        MaximumStrictlyIncreasingCellsInAMatrixSolution.MaxIncreasingCellsByMemoizedRowColumnScan(_matrix);

    [Benchmark]
    public int SortedBatchDp() =>
        MaximumStrictlyIncreasingCellsInAMatrixSolution.MaxIncreasingCellsBySortedBatchDp(_matrix);
}
