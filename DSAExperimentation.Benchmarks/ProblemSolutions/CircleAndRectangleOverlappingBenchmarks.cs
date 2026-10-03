using DSAExperimentation.LeetCode.CircleAndRectangleOverlapping;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CircleAndRectangleOverlappingSolution's, the same methods
// CircleAndRectangleOverlappingSolutionTests proves correct - the brute-force O(width*height)
// lattice-point scan against the O(1) closed-form clamp-and-distance check. The circle
// has LC 1401's smallest radius, 1, and is centred one unit past the rectangle's far
// corner, so it only overlaps at that single last-scanned lattice point, forcing the
// brute-force scan through its full worst case instead of exiting early.
public class CircleAndRectangleOverlappingBenchmarks
{
    private int _radius;

    private int _xCenter, _yCenter;
    private int _x1, _y1, _x2, _y2;
    [Params(60, 400)]
    public int Side { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _x1 = 0;
        _y1 = 0;
        _x2 = Side;
        _y2 = Side;

        _xCenter = Side + 1;
        _yCenter = Side;
        _radius = 1;
    }

    [Benchmark(Baseline = true)]
    public bool HasOverlapByLatticePointScan() =>
        CircleAndRectangleOverlappingSolution.HasOverlapByLatticePointScan(
            new Circle(_radius, _xCenter, _yCenter), new Rectangle(_x1, _y1, _x2, _y2));

    [Benchmark]
    public bool HasOverlapByClampedDistance() =>
        CircleAndRectangleOverlappingSolution.HasOverlapByClampedDistance(
            new Circle(_radius, _xCenter, _yCenter), new Rectangle(_x1, _y1, _x2, _y2));
}
