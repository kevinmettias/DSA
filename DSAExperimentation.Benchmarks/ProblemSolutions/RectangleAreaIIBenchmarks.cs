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
    [Params(20, 300)]
    public int RectangleCount;

    private int[][] _rectangles = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(7);
        _rectangles = new int[RectangleCount][];

        for (var i = 0; i < RectangleCount; i++)
        {
            var x1 = random.Next(0, RectangleCount * 2);
            var y1 = random.Next(0, RectangleCount * 2);
            _rectangles[i] = [x1, y1, x1 + random.Next(1, 10), y1 + random.Next(1, 10)];
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
            var x1 = xs[i];
            var x2 = xs[i + 1];
            var yIntervals = new IntervalSet<int>();

            foreach (var rectangle in _rectangles)
            {
                if (rectangle[0] <= x1 && rectangle[2] >= x2)
                {
                    yIntervals.Add(rectangle[1], rectangle[3]);
                }
            }

            for (var j = 0; j < yIntervals.Count; j++)
            {
                var (start, end) = yIntervals.Get(j);
                area += (long)(x2 - x1) * (end - start);
            }
        }

        return area;
    }

    private bool IsCovered(int x1, int x2, int y1, int y2)
    {
        foreach (var rectangle in _rectangles)
        {
            if (rectangle[0] <= x1 && rectangle[2] >= x2 && rectangle[1] <= y1 && rectangle[3] >= y2)
            {
                return true;
            }
        }

        return false;
    }

    private int[] DistinctSorted(int axis)
        => _rectangles.SelectMany(r => new[] { r[axis], r[axis + 2] }).Distinct().OrderBy(x => x).ToArray();
}
