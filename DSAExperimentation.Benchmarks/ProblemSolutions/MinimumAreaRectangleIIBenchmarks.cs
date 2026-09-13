using BenchmarkDotNet.Attributes;
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

    [Params(12, 24)]
    public int Length;

    private int[][] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var grid = (int)Math.Ceiling(Math.Sqrt(Length)) + GridPadding;

        var coordinates = new HashSet<(int X, int Y)>();

        while (coordinates.Count < Length)
        {
            coordinates.Add((random.Next(grid), random.Next(grid)));
        }

        _points = coordinates.Select(c => new[] { c.X, c.Y }).ToArray();
    }

    [Benchmark(Baseline = true)]
    public double BruteForceQuadruples() =>
        MinimumAreaRectangleIISolution.MinAreaFreeRectByQuadrupleScan(_points);

    [Benchmark]
    public double DiagonalGrouping() =>
        MinimumAreaRectangleIISolution.MinAreaFreeRectByDiagonalGrouping(_points);
}
