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
    public long UnitGridCoverageCount()
    {
        var rectA = new Rectangle(_ax1, _ay1, _ax2, _ay2);
        var rectB = new Rectangle(_bx1, _by1, _bx2, _by2);
        var frame = ComputeGridFrame(rectA, rectB);
        var covered = InitializeCoverageGrid(frame);

        MarkRectangle(covered, frame, rectA);
        MarkRectangle(covered, frame, rectB);

        return CountCovered(covered);
    }

    private static GridFrame ComputeGridFrame(Rectangle a, Rectangle b)
    {
        var minX = Math.Min(a.X1, b.X1);
        var minY = Math.Min(a.Y1, b.Y1);
        var maxX = Math.Max(a.X2, b.X2);
        var maxY = Math.Max(a.Y2, b.Y2);

        return new GridFrame(maxX - minX, maxY - minY, minX, minY);
    }

    private static DynamicArray<bool> InitializeCoverageGrid(GridFrame frame)
    {
        var covered = new DynamicArray<bool>();

        for (var i = 0; i < frame.Width * frame.Height; i++)
        {
            covered.Add(false);
        }

        return covered;
    }

    private static long CountCovered(DynamicArray<bool> covered)
    {
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

    private static void MarkRectangle(DynamicArray<bool> covered, GridFrame frame, Rectangle rect)
    {
        for (var y = rect.Y1; y < rect.Y2; y++)
        {
            for (var x = rect.X1; x < rect.X2; x++)
            {
                covered.Set((y - frame.MinY) * frame.Width + (x - frame.MinX), true);
            }
        }
    }

    private readonly record struct Rectangle(int X1, int Y1, int X2, int Y2);

    private readonly record struct GridFrame(int Width, int Height, int MinX, int MinY);
}
