using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Rectangle Area II (LC 850): CoordinateCompressionCellCheck compresses x and y
// into O(n) distinct coordinates each, then checks every one of the O(n^2)
// resulting cells against every rectangle to decide if it's covered, O(n^3).
// SweepLineWithIntervalSet instead walks the O(n) x-slabs once and merges each
// slab's active rectangles' y-ranges with this repo's own IntervalSet<TKey>,
// O(n^2 log n) - one IntervalSet.Add per active rectangle per slab instead of
// one cell-membership scan per grid cell.
[MemoryDiagnoser]
public class RectangleAreaIIBenchmarks
{
    private const int RandomSeed = 7;
    private const int CoordinateRangeMultiplier = 2;
    private const int MaxRectangleDimension = 10;
    private const int X2Index = 2;
    private const int Y2Index = 3;
    private const int FarCoordinateOffset = 2;

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
    public long CoordinateCompressionCellCheck()
    {
        var xs = DistinctSorted(axis: 0);
        var ys = DistinctSorted(axis: 1);
        long area = 0;

        for (var i = 0; i < xs.Length - 1; i++)
        {
            for (var j = 0; j < ys.Length - 1; j++)
            {
                if (IsCovered(xs[i], xs[i + 1], ys[j], ys[j + 1]))
                {
                    area += (long)(xs[i + 1] - xs[i]) * (ys[j + 1] - ys[j]);
                }
            }
        }

        return area;
    }

    [Benchmark]
    public long SweepLineWithIntervalSet()
    {
        var xs = DistinctSorted(axis: 0);
        long area = 0;

        for (var i = 0; i < xs.Length - 1; i++)
        {
            area += ComputeSlabArea(xs[i], xs[i + 1]);
        }

        return area;
    }

    private long ComputeSlabArea(int x1, int x2)
    {
        var yIntervals = new IntervalSet<int>();

        foreach (var rectangle in _rectangles)
        {
            if (rectangle[0] <= x1 && rectangle[X2Index] >= x2)
            {
                yIntervals.Add(rectangle[1], rectangle[Y2Index]);
            }
        }

        long slabArea = 0;

        for (var j = 0; j < yIntervals.Count; j++)
        {
            var (start, end) = yIntervals.Get(j);
            slabArea += (long)(x2 - x1) * (end - start);
        }

        return slabArea;
    }

    private bool IsCovered(int x1, int x2, int y1, int y2)
    {
        foreach (var rectangle in _rectangles)
        {
            if (rectangle[0] <= x1 && rectangle[X2Index] >= x2 && rectangle[1] <= y1 && rectangle[Y2Index] >= y2)
            {
                return true;
            }
        }

        return false;
    }

    private int[] DistinctSorted(int axis)
        => _rectangles.SelectMany(r => new[] { r[axis], r[axis + FarCoordinateOffset] }).Distinct().OrderBy(x => x).ToArray();
}
