using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SelectCellsInGridWithMaximumScore;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SelectCellsInGridWithMaximumScoreSolution's, the
// same methods SelectCellsInGridWithMaximumScoreSolutionTests proves correct. The bitmask
// arm is handed the prepared rows-by-value grouping its hoisted overload takes, so
// that grouping is charged to [GlobalSetup] rather than the memoized DP being
// measured.
//
// Sizes are per arm, each a square grid's side. The brute force tries every distinct
// value in every row, roughly side^side paths, so it stops at 7; the bitmask DP solves
// each (value, row mask) state once, at most side * 2^side of them, and runs on to
// LC 3276's own bound of a 10 x 10 grid. The two are compared at the sizes both run.
public class SelectCellsInGridWithMaximumScoreBenchmarks
{
    private const int Seed = 3276;

    private Dictionary<int, (int[][] Grid, Dictionary<int, List<int>> RowsByValue)> _workloadBySize = [];

    public static IEnumerable<int> BaselineSizes => [4, 7];

    public static IEnumerable<int> BitmaskSizes => [.. BaselineSizes, 10];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _workloadBySize = BitmaskSizes.ToDictionary(gridSize => gridSize, BuildWorkload);

    private static (int[][] Grid, Dictionary<int, List<int>> RowsByValue) BuildWorkload(int gridSize)
    {
        var grid = SelectCellsWorkloads.BuildGrid(gridSize, Seed);

        return (grid, SelectCellsInGridWithMaximumScoreSolution.GroupRowsByValue(grid));
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int BruteForceRecursion(int gridSize) =>
        SelectCellsInGridWithMaximumScoreSolution.MaxScoreByBruteForceRecursion(_workloadBySize[gridSize].Grid);

    [Benchmark]
    [ArgumentsSource(nameof(BitmaskSizes))]
    public int BitmaskMemoization(int gridSize) =>
        SelectCellsInGridWithMaximumScoreSolution.MaxScoreByBitmaskMemoization(_workloadBySize[gridSize].RowsByValue);
}
