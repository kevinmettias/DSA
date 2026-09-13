using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RectangleAreaII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RectangleAreaIISolution's, the same methods
// RectangleAreaIITests proves correct. CoordinateCompressionCellCheck compresses
// x and y into O(n) distinct coordinates each and checks every one of the O(n^2)
// resulting cells against every rectangle, O(n^3); SweepLineWithIntervalSet
// walks the O(n) x-slabs once and merges each slab's active rectangles' y-ranges
// with this repo's own IntervalSet<TKey>, O(n^2 log n). Coordinate compression
// is part of what each arm is being measured on, so only the rectangle workload
// itself is built in [GlobalSetup].
[MemoryDiagnoser]
public class RectangleAreaIIBenchmarks
{
    private const int RandomSeed = 7;
    private const int CoordinateRangeMultiplier = 2;
    private const int MaxRectangleDimension = 10;

    [Params(20, 300)]
    public int RectangleCount;

    private int[][] _rectangles = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _rectangles = new int[RectangleCount][];

        for (var i = 0; i < RectangleCount; i++)
        {
            var x1 = random.Next(0, RectangleCount * CoordinateRangeMultiplier);
            var y1 = random.Next(0, RectangleCount * CoordinateRangeMultiplier);
            _rectangles[i] = [x1, y1, x1 + random.Next(1, MaxRectangleDimension), y1 + random.Next(1, MaxRectangleDimension)];
        }
    }

    [Benchmark(Baseline = true)]
    public int CoordinateCompressionCellCheck() =>
        RectangleAreaIISolution.TotalAreaByCoordinateCompression(_rectangles);

    [Benchmark]
    public int SweepLineWithIntervalSet() =>
        RectangleAreaIISolution.TotalAreaBySweepLine(_rectangles);
}
