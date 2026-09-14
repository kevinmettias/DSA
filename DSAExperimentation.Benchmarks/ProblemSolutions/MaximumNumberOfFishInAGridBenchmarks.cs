using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumNumberOfFishInAGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumNumberOfFishInAGridSolution's, the same
// methods MaximumNumberOfFishInAGridTests proves correct - a hand-specialized
// recursive flood fill (the textbook approach) vs. this repo's own
// DepthFirstSearch.Traverse walking each water component's reachable cells, the
// same contrast MaxAreaOfIslandBenchmarks draws for LC 695. Each strategy clones
// the shared grid fixture internally before sinking cells, so repeated benchmark
// invocations each start from the true input.
[MemoryDiagnoser]
public class MaximumNumberOfFishInAGridBenchmarks
{
    private const int RandomSeed = 2658; // LC problem number
    private const double WaterProbability = 0.55;
    private const int MaxFishExclusive = 11;

    [Params(30, 120)]
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
                _grid[r][c] = random.NextDouble() < WaterProbability ? random.Next(1, MaxFishExclusive) : 0;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveRecursiveFloodFill() =>
        MaximumNumberOfFishInAGridSolution.MaxFishByNaiveFloodFill(_grid);

    [Benchmark]
    public int DepthFirstSearchTraversal() =>
        MaximumNumberOfFishInAGridSolution.MaxFishByDepthFirstSearch(_grid);
}
