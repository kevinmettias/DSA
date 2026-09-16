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

    private Rectangle _firstRectangle;

    private Rectangle _secondRectangle;
    [Params(60, 400)]
    public int Side { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _firstRectangle = new Rectangle(0, 0, Side, Side);

        var offset = Side / OffsetDivisor;
        _secondRectangle = new Rectangle(offset, offset, offset + Side, offset + Side);
    }

    [Benchmark(Baseline = true)]
    public long UnitGridCoverageCount() =>
        RectangleAreaSolution.TotalAreaByUnitGridCoverageCount(_firstRectangle, _secondRectangle);

    [Benchmark]
    public long ClosedFormOverlapArithmetic() =>
        RectangleAreaSolution.TotalAreaByClosedFormOverlap(_firstRectangle, _secondRectangle);
}
