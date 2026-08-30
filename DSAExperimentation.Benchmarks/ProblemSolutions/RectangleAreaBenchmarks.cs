using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Rectangle Area (LC 223): brute-force O(width*height) unit-grid coverage counting,
// using this repo's own DynamicArray<bool> as the coverage grid (the same role it
// plays as CountPrimesBenchmarks's sieve array), vs. the O(1) closed-form overlap
// arithmetic. Rectangle 2 is offset by half of rectangle 1's side along both axes,
// so the two always overlap and the overlap region scales with Side, keeping both
// strategies honest as Side grows.
[MemoryDiagnoser]
public class RectangleAreaBenchmarks
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
    public long UnitGridCoverageCount()
    {
        var minX = Math.Min(_ax1, _bx1);
        var minY = Math.Min(_ay1, _by1);
        var maxX = Math.Max(_ax2, _bx2);
        var maxY = Math.Max(_ay2, _by2);
        var width = maxX - minX;
        var height = maxY - minY;

        var covered = new DynamicArray<bool>();
        for (var i = 0; i < width * height; i++)
        {
            covered.Add(false);
        }

        MarkRectangle(covered, width, minX, minY, _ax1, _ay1, _ax2, _ay2);
        MarkRectangle(covered, width, minX, minY, _bx1, _by1, _bx2, _by2);

        long count = 0;
        for (var i = 0; i < covered.Count; i++)
        {
            if (covered.Get(i))
            {
                count++;
            }
        }

        return count;
    }

    [Benchmark]
    public long ClosedFormOverlapArithmetic()
    {
        var area1 = (long)(_ax2 - _ax1) * (_ay2 - _ay1);
        var area2 = (long)(_bx2 - _bx1) * (_by2 - _by1);

        var overlapWidth = Math.Max(0, Math.Min(_ax2, _bx2) - Math.Max(_ax1, _bx1));
        var overlapHeight = Math.Max(0, Math.Min(_ay2, _by2) - Math.Max(_ay1, _by1));

        return area1 + area2 - (long)overlapWidth * overlapHeight;
    }

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
