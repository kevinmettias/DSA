using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Perfect Rectangle (LC 391): O(n^2) brute-force pairwise overlap checking (plus the
// same O(n) area-vs-bounding-box gap check both approaches still need) vs. O(n)
// corner-toggle counting via this repo's own Set<(int,int)> (composing
// HashMap<T,bool>). The input is a genuine GridSize x GridSize unit-square tiling - a
// real perfect cover, not a rejected one - so neither approach short-circuits early
// on a detected overlap and both run their full worst-case pass.
[MemoryDiagnoser]
public class PerfectRectangleBenchmarks
{
    private const int X2Index = 2;
    private const int Y2Index = 3;
    private const int PerfectRectangleCornerCount = 4;

    [Params(20, 150)]
    public int GridSize;

    private int[][] _rectangles = [];

    [GlobalSetup]
    public void Setup()
    {
        var rectangles = new List<int[]>();

        for (var x = 0; x < GridSize; x++)
        {
            for (var y = 0; y < GridSize; y++)
            {
                rectangles.Add([x, y, x + 1, y + 1]);
            }
        }

        _rectangles = rectangles.ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool PairwiseOverlapCheck() => IsRectangleCoverBruteForce(_rectangles);

    [Benchmark]
    public bool CornerToggleSet() => IsRectangleCoverWithSet(_rectangles);

    private static bool IsRectangleCoverBruteForce(int[][] rectangles)
    {
        for (var i = 0; i < rectangles.Length; i++)
        {
            for (var j = i + 1; j < rectangles.Length; j++)
            {
                if (Overlaps(rectangles[i], rectangles[j]))
                {
                    return false;
                }
            }
        }

        return AreaMatchesBoundingBox(rectangles);
    }

    private static bool Overlaps(int[] a, int[] b)
        => a[0] < b[X2Index] && b[0] < a[X2Index] && a[1] < b[Y2Index] && b[1] < a[Y2Index];

    private static bool AreaMatchesBoundingBox(int[][] rectangles)
    {
        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;
        long totalArea = 0;

        foreach (var rect in rectangles)
        {
            minX = Math.Min(minX, rect[0]);
            minY = Math.Min(minY, rect[1]);
            maxX = Math.Max(maxX, rect[X2Index]);
            maxY = Math.Max(maxY, rect[Y2Index]);
            totalArea += (long)(rect[X2Index] - rect[0]) * (rect[Y2Index] - rect[1]);
        }

        return totalArea == (long)(maxX - minX) * (maxY - minY);
    }

    private static bool IsRectangleCoverWithSet(int[][] rectangles)
    {
        var bounds = new BoundingBoxAccumulator();
        var corners = new Set<(int X, int Y)>();

        foreach (var rect in rectangles)
        {
            AccumulateRectangle(rect, corners, bounds);
        }

        if (bounds.TotalArea != (long)(bounds.MaxX - bounds.MinX) * (bounds.MaxY - bounds.MinY) || corners.Count != PerfectRectangleCornerCount)
        {
            return false;
        }

        return corners.Has((bounds.MinX, bounds.MinY)) && corners.Has((bounds.MinX, bounds.MaxY))
            && corners.Has((bounds.MaxX, bounds.MinY)) && corners.Has((bounds.MaxX, bounds.MaxY));
    }

    private static void AccumulateRectangle(int[] rect, Set<(int X, int Y)> corners, BoundingBoxAccumulator bounds)
    {
        var (x1, y1, x2, y2) = (rect[0], rect[1], rect[X2Index], rect[Y2Index]);
        bounds.MinX = Math.Min(bounds.MinX, x1);
        bounds.MinY = Math.Min(bounds.MinY, y1);
        bounds.MaxX = Math.Max(bounds.MaxX, x2);
        bounds.MaxY = Math.Max(bounds.MaxY, y2);
        bounds.TotalArea += (long)(x2 - x1) * (y2 - y1);

        ToggleCorner(corners, (x1, y1));
        ToggleCorner(corners, (x1, y2));
        ToggleCorner(corners, (x2, y1));
        ToggleCorner(corners, (x2, y2));
    }

    private static void ToggleCorner(Set<(int X, int Y)> corners, (int X, int Y) point)
    {
        if (!corners.TryAdd(point))
        {
            corners.TryRemove(point);
        }
    }

    private sealed class BoundingBoxAccumulator
    {
        public int MinX = int.MaxValue;
        public int MinY = int.MaxValue;
        public int MaxX = int.MinValue;
        public int MaxY = int.MinValue;
        public long TotalArea;
    }
}
