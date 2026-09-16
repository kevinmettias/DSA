using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumAreaRectangleII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumAreaRectangleIISolution's, the same methods
// MinimumAreaRectangleIITests proves correct. The points are drawn from a grid
// barely larger than the point count, so rectangles - rotated ones included - are
// plentiful and both arms do real work. Length is kept modest because the
// quadruple-scan baseline is O(n^4), the same reasoning
// MinimumAreaRectangleBenchmarks' own cubic baseline documents. Point construction
// is charged to [GlobalSetup].
[MemoryDiagnoser]
public class MinimumAreaRectangleIIBenchmarks
{
    private const int RandomSeed = 963; // LC 963
    private const int GridPadding = 3;

    private int[][] _points = [];

    [Params(12, 24)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _points = LatticePointWorkloads.InGrid(Length, GridPadding, RandomSeed);

    [Benchmark(Baseline = true)]
    public double BruteForceQuadruples() =>
        MinimumAreaRectangleIISolution.MinAreaFreeRectByQuadrupleScan(_points);

    [Benchmark]
    public double DiagonalGrouping() =>
        MinimumAreaRectangleIISolution.MinAreaFreeRectByDiagonalGrouping(_points);
}
