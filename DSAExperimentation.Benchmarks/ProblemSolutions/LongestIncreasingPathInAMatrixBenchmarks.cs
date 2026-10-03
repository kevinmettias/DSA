using DSAExperimentation.LeetCode.LongestIncreasingPathInAMatrix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestIncreasingPathInAMatrixSolution's, the same
// methods LongestIncreasingPathInAMatrixSolutionTests proves correct, run over a strictly
// row-major-increasing Size x Size matrix (so every cell's only increasing neighbors
// are right/down, the classic Unique-Paths-shaped DAG with heavy path overlap).
// NaiveRecursion re-explores every shared sub-path from scratch per candidate start
// cell - central-Delannoy-number growth. MemoizedRecurrence caches sub-paths revisited
// within one start's search, collapsing that call from exponential to polynomial.
//
// Sizes are per arm. The naive arm stops at a 9 x 9 matrix; the memoized arm, which
// pays O(n^2) per start cell and so O(n^4) in all, runs on to 40 x 40 - LC 329 allows
// 200 x 200, but at O(n^4) that is far past a benchmark's budget. The two are compared
// at the sizes both run.
public class LongestIncreasingPathInAMatrixBenchmarks
{
    private Dictionary<int, int[,]> _matrixBySize = [];

    public static IEnumerable<int> NaiveSizes => [6, 9];

    public static IEnumerable<int> MemoizedSizes => [.. NaiveSizes, 20, 40];

    // Every size any arm runs is built here, outside the timed region; an arm looks its
    // own up.
    [GlobalSetup]
    public void Setup() => _matrixBySize = MemoizedSizes.ToDictionary(size => size, BuildRowMajorMatrix);

    private static int[,] BuildRowMajorMatrix(int size)
    {
        var matrix = new int[size, size];

        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                matrix[row, col] = row * size + col;
            }
        }

        return matrix;
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(NaiveSizes))]
    public int NaiveRecursion(int size) =>
        LongestIncreasingPathInAMatrixSolution.LongestPathByNaiveRecursion(_matrixBySize[size]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int MemoizedRecurrence(int size) =>
        LongestIncreasingPathInAMatrixSolution.LongestPathByMemoizedRecurrence(_matrixBySize[size]);
}
