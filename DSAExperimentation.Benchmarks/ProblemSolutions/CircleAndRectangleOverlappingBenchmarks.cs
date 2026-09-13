using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CircleAndRectangleOverlapping;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CircleAndRectangleOverlappingSolution's, the same methods
// CircleAndRectangleOverlappingTests proves correct - the brute-force O(width*height)
// lattice-point scan against the O(1) closed-form clamp-and-distance check. The circle
// is centred exactly on the rectangle's far corner with radius 0, so it only overlaps at
// that single last-scanned lattice point, forcing the brute-force scan through its full
// worst case instead of exiting early.
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
    public bool LatticePointScan() =>
        CircleAndRectangleOverlappingSolution.CheckOverlapByLatticePointScan(
            _radius, _xCenter, _yCenter, _x1, _y1, _x2, _y2);

    [Benchmark]
    public bool ClosedFormClampAndDistance() =>
        CircleAndRectangleOverlappingSolution.CheckOverlapByClampedDistance(
            _radius, _xCenter, _yCenter, _x1, _y1, _x2, _y2);
}
