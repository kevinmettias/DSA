using DSAExperimentation.LeetCode.NumberOfIncreasingPathsInAGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfIncreasingPathsInAGridSolution's, the same
// methods NumberOfIncreasingPathsInAGridSolutionTests proves correct, run over the same
// row-major strictly-increasing Size x Size shape
// LongestIncreasingPathInAMatrixBenchmarks uses (every cell's only increasing
// neighbors are right/down, the classic Unique-Paths-shaped DAG with heavy path
// overlap), numbered from 1 because LC 2328's cells start there. NaiveRecursion re-walks every shared sub-path from scratch per candidate
// start cell - here the call count itself, not just the returned value, grows with the
// number of increasing paths. MemoizedRecurrence dogfoods this repo's own Memoizer per
// start cell, collapsing shared sub-paths within one start's search from exponential
// to polynomial.
//
// Sizes are per arm. The naive arm stops at a side of 9; the memoized arm, whose fresh
// memo per start cell makes it O(side^4) here, runs on to a side of 32, well inside
// LC 2328's 10^5 cells. The two are compared at the sizes both run.
public class NumberOfIncreasingPathsInAGridBenchmarks
{
    private Dictionary<int, int[,]> _matrixBySize = [];

    public static IEnumerable<int> BaselineSizes => [6, 9];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 20, 32];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _matrixBySize = MemoizedSizes.ToDictionary(size => size, BuildMatrix);

    private static int[,] BuildMatrix(int size)
    {
        var matrix = new int[size, size];

        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                matrix[row, col] = (row * size) + col + 1;
            }
        }

        return matrix;
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int NaiveRecursion(int size) =>
        NumberOfIncreasingPathsInAGridSolution.CountPathsByNaiveRecursion(_matrixBySize[size]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int MemoizedRecurrence(int size) =>
        NumberOfIncreasingPathsInAGridSolution.CountPathsByMemoizedRecurrence(_matrixBySize[size]);
}
