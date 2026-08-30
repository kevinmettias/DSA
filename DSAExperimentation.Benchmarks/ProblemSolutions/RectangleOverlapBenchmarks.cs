using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Rectangle Overlap (LC 836): brute-force O(width*height) unit-grid intersection
// scan, using this repo's own DynamicArray<bool> as the coverage grid (the same
// role it plays in RectangleAreaBenchmarks for LC 223), vs. the O(1) closed-form
// axis-overlap check. Rectangle 2 is offset by half of rectangle 1's side along
// both axes, so the two always overlap and the shared region scales with Side.
[MemoryDiagnoser]
public class RectangleOverlapBenchmarks
{
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

        _bx1 = Side / 2;
        _by1 = Side / 2;
        _bx2 = _bx1 + Side;
        _by2 = _by1 + Side;
    }

    [Benchmark(Baseline = true)]
    public bool UnitGridIntersectionScan()
    {
        var minX = Math.Min(_ax1, _bx1);
        var minY = Math.Min(_ay1, _by1);
        var maxX = Math.Max(_ax2, _bx2);
        var maxY = Math.Max(_ay2, _by2);
        var width = maxX - minX;
        var height = maxY - minY;

        var coveredByFirst = new DynamicArray<bool>();
        for (var i = 0; i < width * height; i++)
        {
            coveredByFirst.Add(false);
        }

        MarkRectangle(coveredByFirst, width, minX, minY, _ax1, _ay1, _ax2, _ay2);

        for (var y = _by1; y < _by2; y++)
        {
            for (var x = _bx1; x < _bx2; x++)
            {
                if (coveredByFirst.Get((y - minY) * width + (x - minX)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    [Benchmark]
    public bool ClosedFormAxisOverlap()
        => _ax1 < _bx2 && _bx1 < _ax2 && _ay1 < _by2 && _by1 < _ay2;

    private static void MarkRectangle(DynamicArray<bool> covered, int width, int minX, int minY, int x1, int y1, int x2, int y2)
    {
        for (var y = y1; y < y2; y++)
        {
            for (var x = x1; x < x2; x++)
            {
                covered.Set((y - minY) * width + (x - minX), true);
            }
        }
    }
}
