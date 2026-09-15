using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ContainVirus;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ContainVirusSolution's, the same methods
// ContainVirusTests proves correct. Each strategy clones the workload grid itself,
// so [GlobalSetup] only has to build it once per [Params] value rather than per
// iteration.
[MemoryDiagnoser]
public class ContainVirusBenchmarks
{
    private const int RandomSeed = 749; // LeetCode problem number

    private const double InfectionSeedProbability = 0.15;

    private int[][] _grid = [];

    [Params(10, 25)]
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
                var isInfected = random.NextDouble() < InfectionSeedProbability;
                _grid[r][c] = isInfected ? 1 : 0;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveRecursiveFloodFill() => ContainVirusSolution.MinimumWallsByNaiveRecursiveFloodFill(_grid);

    [Benchmark]
    public int DepthFirstSearchTraversal() => ContainVirusSolution.MinimumWallsByDepthFirstSearchTraversal(_grid);
}
