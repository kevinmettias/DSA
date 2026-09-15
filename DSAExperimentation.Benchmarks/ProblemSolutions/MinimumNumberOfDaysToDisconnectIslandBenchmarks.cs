using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNumberOfDaysToDisconnectIsland;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfDaysToDisconnectIslandSolution's, the
// same methods MinimumNumberOfDaysToDisconnectIslandTests proves correct. The
// baseline is a hand-rolled recursive flood fill counting connected land components
// (the textbook approach, no repo primitive) against this repo's own
// DepthFirstSearch.Traverse doing the same count - the same primitive
// MakingALargeIslandBenchmarks/NumberOfIslandsBenchmarks already use for grid
// connectivity.
//
// The workload is a solid all-land square: a full rectangular grid graph has no
// articulation cell (removing any single land cell never disconnects it), so MinDays
// always returns 2 - forcing both arms through their full O((Rows*Cols)^2) worst case,
// one connectivity check per candidate removal, rather than an early exit after the
// first cell tried.
[MemoryDiagnoser]
public class MinimumNumberOfDaysToDisconnectIslandBenchmarks
{
    private const int Land = 1;

    private int[][] _grid = [];

    [Params(10, 20)]
    public int Side { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _grid = new int[Side][];

        for (var row = 0; row < Side; row++)
        {
            _grid[row] = Enumerable.Repeat(Land, Side).ToArray();
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveRecursiveFloodFill()
        => MinimumNumberOfDaysToDisconnectIslandSolution.MinDaysByNaiveFloodFill(_grid);

    [Benchmark]
    public int PrimitiveComposed()
        => MinimumNumberOfDaysToDisconnectIslandSolution.MinDaysByDepthFirstSearch(_grid);
}
