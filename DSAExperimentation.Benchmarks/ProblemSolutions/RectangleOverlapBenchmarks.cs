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
    public bool UnitGridIntersectionScan()
    {
        var rectA = new Rectangle(_ax1, _ay1, _ax2, _ay2);
        var rectB = new Rectangle(_bx1, _by1, _bx2, _by2);
        var frame = ComputeGridFrame(rectA, rectB);
        var coveredByFirst = InitializeCoverageGrid(frame);

        MarkRectangle(coveredByFirst, frame, rectA);

        return IntersectsMarkedGrid(coveredByFirst, frame, rectB);
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

    private static bool IntersectsMarkedGrid(DynamicArray<bool> covered, GridFrame frame, Rectangle rect)
    {
        for (var y = rect.Y1; y < rect.Y2; y++)
        {
            for (var x = rect.X1; x < rect.X2; x++)
            {
                if (covered.Get((y - frame.MinY) * frame.Width + (x - frame.MinX)))
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
