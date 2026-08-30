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
        => a[0] < b[2] && b[0] < a[2] && a[1] < b[3] && b[1] < a[3];

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
            maxX = Math.Max(maxX, rect[2]);
            maxY = Math.Max(maxY, rect[3]);
            totalArea += (long)(rect[2] - rect[0]) * (rect[3] - rect[1]);
        }

        return totalArea == (long)(maxX - minX) * (maxY - minY);
    }

    private static bool IsRectangleCoverWithSet(int[][] rectangles)
    {
        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;
        long totalArea = 0;
        var corners = new Set<(int X, int Y)>();

        foreach (var rect in rectangles)
        {
            var (x1, y1, x2, y2) = (rect[0], rect[1], rect[2], rect[3]);
            minX = Math.Min(minX, x1);
            minY = Math.Min(minY, y1);
            maxX = Math.Max(maxX, x2);
            maxY = Math.Max(maxY, y2);
            totalArea += (long)(x2 - x1) * (y2 - y1);

            ToggleCorner(corners, (x1, y1));
            ToggleCorner(corners, (x1, y2));
            ToggleCorner(corners, (x2, y1));
            ToggleCorner(corners, (x2, y2));
        }

        if (totalArea != (long)(maxX - minX) * (maxY - minY) || corners.Count != 4)
        {
            return false;
        }

        return corners.Has((minX, minY)) && corners.Has((minX, maxY))
            && corners.Has((maxX, minY)) && corners.Has((maxX, maxY));
    }

    private static void ToggleCorner(Set<(int X, int Y)> corners, (int X, int Y) point)
    {
        if (!corners.TryAdd(point))
        {
            corners.TryRemove(point);
        }
    }
}
