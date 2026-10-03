using DSAExperimentation.LeetCode.CheckIfThereIsAValidParenthesesStringPath;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheckIfThereIsAValidParenthesesStringPathSolution's,
// the same methods CheckIfThereIsAValidParenthesesStringPathSolutionTests proves agree.
//
// The grid is all '(' so nothing short-circuits the un-memoized arm's full
// right/down branching early - the balance only ever grows, never goes negative, so
// it really does visit every one of the C(2n-2, n-1) paths. Grid construction is the
// LeetCode input shape itself, so building it in [GlobalSetup] already keeps it off
// the measured methods.
//
// Sizes are per arm. That path count is exponential, so the un-memoized arm stops at
// a 12 x 12 grid; the memoized arm visits each (row, column, balance) state once and
// runs on to LC 2267's own bound of 100 x 100, and the two are compared at the sizes
// both run.
public class CheckIfThereIsAValidParenthesesStringPathBenchmarks
{
    private const char Open = '(';

    private Dictionary<int, char[,]> _gridBySize = [];

    public static IEnumerable<int> UnmemoizedSizes => [8, 12];

    public static IEnumerable<int> MemoizedSizes => [.. UnmemoizedSizes, 25, 100];

    // Every size any arm runs is built here, outside the timed region; an arm looks its
    // own up.
    [GlobalSetup]
    public void Setup() => _gridBySize = MemoizedSizes.ToDictionary(size => size, BuildAllOpenGrid);

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(UnmemoizedSizes))]
    public bool HasValidPathByUnmemoizedRecursion(int size) =>
        CheckIfThereIsAValidParenthesesStringPathSolution.HasValidPathByUnmemoizedRecursion(_gridBySize[size]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public bool HasValidPathByMemoizedRecursion(int size) =>
        CheckIfThereIsAValidParenthesesStringPathSolution.HasValidPathByMemoizedRecursion(_gridBySize[size]);

    private static char[,] BuildAllOpenGrid(int size)
    {
        var grid = new char[size, size];

        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                grid[row, col] = Open;
            }
        }

        return grid;
    }
}
