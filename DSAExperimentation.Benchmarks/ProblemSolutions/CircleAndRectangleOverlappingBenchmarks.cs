using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Circle and Rectangle Overlapping (LC 1401): brute-force O(width*height)
// lattice-point scan, using this repo's own DynamicArray<bool> to record
// each rectangle lattice point's circle membership (the same coverage-grid
// role it plays in RectangleOverlapBenchmarks/RectangleAreaBenchmarks), vs.
// the O(1) closed-form clamp-and-distance check. The circle is centered
// exactly on the rectangle's far corner with radius 0, so it only overlaps
// at that single last-scanned lattice point - forcing the brute-force scan
// through its full worst case instead of exiting early.
[MemoryDiagnoser]
public class CircleAndRectangleOverlappingBenchmarks
{
    [Params(60, 400)]
    public int Side;

    private int _radius;
    private int _xCenter, _yCenter;
    private int _x1, _y1, _x2, _y2;

    [GlobalSetup]
    public void Setup()
    {
        _x1 = 0;
        _y1 = 0;
        _x2 = Side;
        _y2 = Side;

        _xCenter = Side;
        _yCenter = Side;
        _radius = 0;
    }

    [Benchmark(Baseline = true)]
    public bool LatticePointScan()
    {
        var width = _x2 - _x1 + 1;
        var height = _y2 - _y1 + 1;

        var withinCircle = BuildCircleMembershipGrid();

        return AnyPointWithinCircle(withinCircle, width * height);
    }

    private DynamicArray<bool> BuildCircleMembershipGrid()
    {
        var withinCircle = new DynamicArray<bool>();
        for (var y = _y1; y <= _y2; y++)
        {
            for (var x = _x1; x <= _x2; x++)
            {
                var dx = (long)(x - _xCenter);
                var dy = (long)(y - _yCenter);
                withinCircle.Add((dx * dx) + (dy * dy) <= (long)_radius * _radius);
            }
        }

        return withinCircle;
    }

    private static bool AnyPointWithinCircle(DynamicArray<bool> withinCircle, int pointCount)
    {
        for (var i = 0; i < pointCount; i++)
        {
            if (withinCircle.Get(i))
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public bool ClosedFormClampAndDistance()
    {
        var closestX = Math.Clamp(_xCenter, _x1, _x2);
        var closestY = Math.Clamp(_yCenter, _y1, _y2);
        var dx = (long)(_xCenter - closestX);
        var dy = (long)(_yCenter - closestY);
        return (dx * dx) + (dy * dy) <= (long)_radius * _radius;
    }
}
