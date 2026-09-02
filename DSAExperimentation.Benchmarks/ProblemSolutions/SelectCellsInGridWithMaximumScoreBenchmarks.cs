using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SelectCellsInGridWithMaximumScore;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SelectCellsInGridWithMaximumScoreSolution's, the
// same methods SelectCellsInGridWithMaximumScoreTests proves correct. The bitmask
// arm is handed the prepared rows-by-value grouping its hoisted overload takes, so
// that grouping is charged to [GlobalSetup] rather than the memoized DP being
// measured.
[MemoryDiagnoser]
public class SelectCellsInGridWithMaximumScoreBenchmarks
{
    private const int Seed = 3276;

    [Params(4, 7)]
    public int GridSize;

    private int[][] _grid = null!;
    private Dictionary<int, List<int>> _rowsByValue = null!;

    [GlobalSetup]
    public void Setup()
    {
        _grid = SelectCellsWorkloads.BuildGrid(GridSize, Seed);
        _rowsByValue = SelectCellsInGridWithMaximumScoreSolution.GroupRowsByValue(_grid);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() =>
        SelectCellsInGridWithMaximumScoreSolution.MaxScoreByBruteForceRecursion(_grid);

    [Benchmark]
    public int BitmaskMemoization() =>
        SelectCellsInGridWithMaximumScoreSolution.MaxScoreByBitmaskMemoization(_rowsByValue);
}
