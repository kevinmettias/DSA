using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaxAreaOfIsland;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaxAreaOfIslandSolution's, the same methods
// MaxAreaOfIslandTests proves correct - a hand-specialized recursive flood fill
// (the textbook approach) vs. this repo's own DepthFirstSearch.Traverse walking
// each island's reachable land cells. Each strategy clones the shared grid
// fixture internally before mutating it, so repeated benchmark invocations each
// start from the true input.
[MemoryDiagnoser]
public class MaxAreaOfIslandBenchmarks
{
    private const int RandomSeed = 3;
    private const double LandProbability = 0.55;

    private int[][] _grid = [];

    [Params(30, 120)]
    public int Side { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid = new int[Side][];

        for (var r = 0; r < Side; r++)
        {
            _grid[r] = new int[Side];

            for (var c = 0; c < Side; c++)
            {
                var isLand = random.NextDouble() < LandProbability;
                _grid[r][c] = isLand ? 1 : 0;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int ByNaiveFloodFill() => MaxAreaOfIslandSolution.MaxAreaByNaiveFloodFill(_grid);

    [Benchmark]
    public int ByDepthFirstSearch() => MaxAreaOfIslandSolution.MaxAreaByDepthFirstSearch(_grid);
}
