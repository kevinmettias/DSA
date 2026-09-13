using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MakingALargeIsland;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MakingALargeIslandSolution's, the same methods
// MakingALargeIslandTests proves correct - repeatedly flipping each water cell and
// running a fresh recursive flood fill (the textbook brute force) vs. this repo's
// own DepthFirstSearch.Traverse labeling every island exactly once into a
// HashMap<int,int> of id -> area, then summing each water cell's already-known
// neighboring areas (deduped through this repo's own Set<int>) in a single pass.
// Each strategy clones the shared grid fixture internally before labeling it, so
// repeated invocations each start from the true input.
[MemoryDiagnoser]
public class MakingALargeIslandBenchmarks
{
    private const int RandomSeed = 7; // LC problem number
    private const double LandProbability = 0.6;

    [Params(20, 60)]
    public int Side;

    private int[][] _grid = null!;

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
                _grid[r][c] = random.NextDouble() < LandProbability ? 1 : 0;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int LargestIslandByNaiveFloodFill() =>
        MakingALargeIslandSolution.LargestIslandByNaiveFloodFill(_grid);

    [Benchmark]
    public int LargestIslandByLabeledFloodFill() =>
        MakingALargeIslandSolution.LargestIslandByLabeledFloodFill(_grid);
}
