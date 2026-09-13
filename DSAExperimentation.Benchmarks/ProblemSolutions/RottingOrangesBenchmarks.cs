using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.RottingOranges;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RottingOrangesSolution's, the same methods
// RottingOrangesTests proves correct. The baseline re-walks an independent BFS
// outward from every fresh orange - a freshly allocated visited grid and BCL Queue
// each time - while the composed arm seeds one shared frontier, this repo's own
// Queue<TElement>, with every already-rotten orange at once: O(rows*cols) total
// instead of O(rows*cols) work per fresh starting cell.
[MemoryDiagnoser]
public class RottingOrangesBenchmarks
{
    private const int RandomSeed = 1;

    [Params(10, 25)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup() => _grid = RottingOrangesWorkloads.BuildGrid(Size, seed: RandomSeed);

    [Benchmark(Baseline = true)]
    public int PerCellBfs() => RottingOrangesSolution.OrangesRottingByPerCellBfs(_grid);

    [Benchmark]
    public int MultiSourceBfs() => RottingOrangesSolution.OrangesRottingByMultiSourceBfs(_grid);
}
