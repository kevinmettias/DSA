using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountSubIslands;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountSubIslandsSolution's, the same methods
// CountSubIslandsTests proves correct. [GlobalSetup] builds the pair of random
// grids - grid1 land-biased relative to grid2 so a real mix of grid2 islands both
// qualify and fail to qualify as sub-islands - so grid construction is charged to
// setup rather than to the flood fill being measured. Each strategy copies grid2
// internally (its own visited set), which is the same per-call clone both arms
// already paid before the migration.
[MemoryDiagnoser]
public class CountSubIslandsBenchmarks
{
    private const int RandomSeed = 1905; // LC 1905 problem number
    private const double Grid1LandProbability = 0.7;
    private const double Grid2LandProbability = 0.55;

    private int[][] _grid1 = [];

    private int[][] _grid2 = [];
    [Params(30, 120)]
    public int Side { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid1 = new int[Side][];
        _grid2 = new int[Side][];

        for (var row = 0; row < Side; row++)
        {
            _grid1[row] = new int[Side];
            _grid2[row] = new int[Side];

            for (var col = 0; col < Side; col++)
            {
                var grid1IsLand = random.NextDouble() < Grid1LandProbability;
                var grid2IsLand = random.NextDouble() < Grid2LandProbability;

                _grid1[row][col] = grid1IsLand ? 1 : 0;
                _grid2[row][col] = grid2IsLand ? 1 : 0;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int RecursiveFloodFill() => CountSubIslandsSolution.CountByRecursiveFloodFill(_grid1, _grid2);

    [Benchmark]
    public int DepthFirstSearchTraverse() => CountSubIslandsSolution.CountByDepthFirstSearchTraverse(_grid1, _grid2);
}
