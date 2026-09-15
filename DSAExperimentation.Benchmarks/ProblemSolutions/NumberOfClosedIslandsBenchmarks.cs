using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfClosedIslands;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfClosedIslandsSolution's, the same methods
// NumberOfClosedIslandsTests proves correct - a hand-specialized recursive flood
// fill threading a "touched border" flag out by ref (the textbook approach) vs.
// this repo's own DepthFirstSearch.Traverse walking each water component and
// checking whether any cell it came back with lands on the grid's edge. Each
// strategy clones the shared grid fixture internally before filling it, so
// repeated invocations each start from the true input.
[MemoryDiagnoser]
public class NumberOfClosedIslandsBenchmarks
{
    private const int RandomSeed = 7;

    private const double LandDensity = 0.55;

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
                var isLand = random.NextDouble() < LandDensity;
                _grid[r][c] = isLand ? 1 : 0;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int CountClosedIslandsByNaiveFloodFill() =>
        NumberOfClosedIslandsSolution.CountClosedIslandsByNaiveFloodFill(_grid);

    [Benchmark]
    public int CountClosedIslandsByDepthFirstSearch() =>
        NumberOfClosedIslandsSolution.CountClosedIslandsByDepthFirstSearch(_grid);
}
