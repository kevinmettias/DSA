using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfEnclaves;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfEnclavesSolution's, the same methods
// NumberOfEnclavesTests proves correct - a hand-specialized recursive border
// flood fill (the textbook approach) vs. this repo's own
// DepthFirstSearch.Traverse walking each border-connected land component. Each
// strategy clones the shared grid fixture internally before sinking anything, so
// repeated invocations each start from the true input.
[MemoryDiagnoser]
public class NumberOfEnclavesBenchmarks
{
    private const int RandomSeed = 1020; // LeetCode problem number

    private const double LandDensity = 0.55;

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
                _grid[r][c] = random.NextDouble() < LandDensity ? 1 : 0;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveRecursiveFloodFill() => NumberOfEnclavesSolution.NumEnclavesByNaiveFloodFill(_grid);

    [Benchmark]
    public int DepthFirstSearchTraversal() => NumberOfEnclavesSolution.NumEnclavesByDepthFirstSearch(_grid);
}
