using DSAExperimentation.LeetCode.CherryPickup;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CherryPickupSolution's, the same methods
// CherryPickupSolutionTests proves correct. The grid is all-cherries with no obstacles
// so nothing short-circuits the naive baseline's full branching early.
//
// Sizes are per arm. That branching is exponential, so the unmemoized arm stops at a
// 6 x 6 grid; the memoized arm's O(n^3) states run on to LC 741's own bound of
// 50 x 50, and the two are compared at the sizes both run.
public class CherryPickupBenchmarks
{
    private Dictionary<int, int[,]> _gridBySize = [];

    public static IEnumerable<int> UnmemoizedSizes => [4, 6];

    public static IEnumerable<int> MemoizedSizes => [.. UnmemoizedSizes, 20, 50];

    // Every size any arm runs is built here, outside the timed region; an arm looks its
    // own up.
    [GlobalSetup]
    public void Setup() => _gridBySize = MemoizedSizes.ToDictionary(size => size, BuildAllCherryGrid);

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(UnmemoizedSizes))]
    public int UnmemoizedRecursion(int size) => CherryPickupSolution.MaxCherriesByUnmemoizedRecursion(_gridBySize[size]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int MemoizedRecursion(int size) => CherryPickupSolution.MaxCherriesByMemoizedRecursion(_gridBySize[size]);

    private static int[,] BuildAllCherryGrid(int size)
    {
        var grid = new int[size, size];

        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                grid[row, col] = 1;
            }
        }

        return grid;
    }
}
