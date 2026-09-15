using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumAreaRectangleWithPointConstraintsII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MaximumAreaRectangleWithPointConstraintsIISolution's, the same methods
// MaximumAreaRectangleWithPointConstraintsIITests proves correct. The sweep arm
// is handed its prepared, x-then-y sorted Point[] from [GlobalSetup] so sorting
// is never part of the measured sweep - the same reason
// MaximumAreaRectangleWithPointConstraintsIBenchmarks hands its corner-lookup
// arm a pre-built Set. Points are a dense grid, same shape Part I's benchmark
// uses, so both arms have real rectangles to find rather than scanning a mostly
// empty space.
[MemoryDiagnoser]
public class MaximumAreaRectangleWithPointConstraintsIIBenchmarks
{
    private int[] _xCoord = [];

    private int[] _yCoord = [];
    private MaximumAreaRectangleWithPointConstraintsIISolution.Point[] _sortedPoints = [];
    // The quadruple scan is O(n^5), so GridSide has to stay small enough for it
    // to finish - the sweep arm alone could run at far larger n.
    [Params(4, 8)]
    public int GridSide { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var n = GridSide * GridSide;
        _xCoord = new int[n];
        _yCoord = new int[n];
        var index = 0;

        for (var x = 0; x < GridSide; x++)
        {
            for (var y = 0; y < GridSide; y++)
            {
                _xCoord[index] = x;
                _yCoord[index] = y;
                index++;
            }
        }

        _sortedPoints = Enumerable.Range(0, n)
            .Select(i => new MaximumAreaRectangleWithPointConstraintsIISolution.Point(_xCoord[i], _yCoord[i]))
            .OrderBy(p => p.X)
            .ThenBy(p => p.Y)
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public long QuadrupleScan() =>
        MaximumAreaRectangleWithPointConstraintsIISolution.MaxAreaByQuadrupleScan(_xCoord, _yCoord);

    [Benchmark]
    public long SweepSegmentTree() =>
        MaximumAreaRectangleWithPointConstraintsIISolution.MaxAreaBySweepSegmentTree(_sortedPoints);
}
