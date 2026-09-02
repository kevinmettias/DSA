using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RectangleArea;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RectangleAreaSolution's, the same methods
// RectangleAreaTests proves correct. Rectangle 2 is offset by half of rectangle
// 1's side along both axes, so the two always overlap and the overlap region
// scales with Side, keeping both strategies honest as Side grows.
[MemoryDiagnoser]
public class RectangleAreaBenchmarks
{
    private const int OffsetDivisor = 2;

    [Params(60, 400)]
    public int Side;

    private int _ax1, _ay1, _ax2, _ay2;
    private int _bx1, _by1, _bx2, _by2;

    [GlobalSetup]
    public void Setup()
    {
        _ax1 = 0;
        _ay1 = 0;
        _ax2 = Side;
        _ay2 = Side;

        _bx1 = Side / OffsetDivisor;
        _by1 = Side / OffsetDivisor;
        _bx2 = _bx1 + Side;
        _by2 = _by1 + Side;
    }

    [Benchmark(Baseline = true)]
    public long UnitGridCoverageCount() =>
        RectangleAreaSolution.TotalAreaByUnitGridCoverageCount(_ax1, _ay1, _ax2, _ay2, _bx1, _by1, _bx2, _by2);

    [Benchmark]
    public long ClosedFormOverlapArithmetic() =>
        RectangleAreaSolution.TotalAreaByClosedFormOverlap(_ax1, _ay1, _ax2, _ay2, _bx1, _by1, _bx2, _by2);
}
