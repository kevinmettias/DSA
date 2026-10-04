using DSAExperimentation.LeetCode.MaximumStrictlyIncreasingCellsInAMatrix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumStrictlyIncreasingCellsInAMatrixSolution's, the
// same methods MaximumStrictlyIncreasingCellsInAMatrixSolutionTests proves correct.
// MemoizedRowColumnScan is the textbook DAG-DP baseline - each of the rows*cols cells
// rescans a whole row plus a whole column, O(rows*cols*(rows+cols)) - while
// SortedBatchDp MergeSorts every cell by value once and folds each equal-value batch
// into running rowBest/colBest arrays, O(rows*cols*log(rows*cols)) with no per-cell
// rescan, so the gap widens with the side length.
//
// Sizes are per arm. SortedBatchDp runs on to a 316 x 316 matrix, 99,856 cells, the
// largest square inside LC 2713's m * n <= 10^5; the rescan stops at 100 x 100. Values
// are drawn from [1, 1,000), inside LeetCode's [-10^5, 10^5]; the narrow range also caps
// the longest increasing walk, and with it the baseline's recursion depth, below 1,000.
//
// Both arms take LeetCode's own jagged matrix, which is already the prepared input, so
// there is nothing for [GlobalSetup] to hoist beyond generating it (#17.4).
public class MaximumStrictlyIncreasingCellsInAMatrixBenchmarks
{
    private const int RandomSeed = 2713; // LC problem number
    private const int ValueRange = 1_000;

    private Dictionary<int, int[][]> _matrixBySide = [];

    public static IEnumerable<int> RescanSides => [8, 20, 100];

    public static IEnumerable<int> SortedBatchSides => [.. RescanSides, 316];

    // Every side any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _matrixBySide = RescanSides.Union(SortedBatchSides).ToDictionary(side => side, BuildMatrix);

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(RescanSides))]
    public int MemoizedRowColumnScan(int side) =>
        MaximumStrictlyIncreasingCellsInAMatrixSolution.MaxIncreasingCellsByMemoizedRowColumnScan(_matrixBySide[side]);

    [Benchmark]
    [ArgumentsSource(nameof(SortedBatchSides))]
    public int SortedBatchDp(int side) =>
        MaximumStrictlyIncreasingCellsInAMatrixSolution.MaxIncreasingCellsBySortedBatchDp(_matrixBySide[side]);

    private static int[][] BuildMatrix(int side)
    {
        var random = new Random(RandomSeed);
        var matrix = new int[side][];

        for (var r = 0; r < side; r++)
        {
            matrix[r] = new int[side];

            for (var c = 0; c < side; c++)
            {
                matrix[r][c] = random.Next(1, ValueRange);
            }
        }

        return matrix;
    }
}
