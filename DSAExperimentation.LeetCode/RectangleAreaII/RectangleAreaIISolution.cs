using DSAExperimentation.DataStructures.IntervalSet;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.RectangleAreaII;

// LeetCode 850. Rectangle Area II: the area of the union of axis-aligned
// rectangles, reported modulo 1e9+7.
//
// Both strategies compress the x-coordinates to the O(n) distinct values where
// the covered set can change, and both report LeetCode's modular answer; they
// differ in what they do with a slab once they have one.
internal static class RectangleAreaIISolution
{
    private const int X1 = 0;
    private const int Y1 = 1;
    private const int X2 = 2;
    private const int Y2 = 3;

    // The textbook answer: compress both axes, then ask of every one of the
    // O(n^2) resulting cells whether some rectangle covers it, O(n^3). BCL
    // arrays and LINQ only - it is the arm the sweep below has to justify itself
    // against. The modulus is LeetCode's reporting convention rather than part
    // of the algorithm, so it comes from Domain.Modular like every other
    // counting problem's.
    public static int TotalAreaByCoordinateCompression(int[][] rectangles)
    {
        var xs = DistinctSorted(rectangles, X1, X2);
        var ys = DistinctSorted(rectangles, Y1, Y2);
        long area = 0;

        for (var i = 0; i < xs.Length - 1; i++)
        {
            area = (area + CoveredRowArea(rectangles, ys, xs[i], xs[i + 1])) % ModularArithmetic.Modulo;
        }

        return (int)area;
    }

    private static long CoveredRowArea(int[][] rectangles, int[] ys, int x1, int x2)
    {
        long height = 0;

        for (var j = 0; j < ys.Length - 1; j++)
        {
            if (IsCovered(rectangles, x1, x2, ys[j], ys[j + 1]))
            {
                height += ys[j + 1] - ys[j];
            }
        }

        return (x2 - x1) * (height % ModularArithmetic.Modulo);
    }

    private static bool IsCovered(int[][] rectangles, int x1, int x2, int y1, int y2)
    {
        foreach (var rectangle in rectangles)
        {
            if (rectangle[X1] <= x1 && rectangle[X2] >= x2 &&
                rectangle[Y1] <= y1 && rectangle[Y2] >= y2)
            {
                return true;
            }
        }

        return false;
    }

    // A sweep line over the distinct x-coordinates: at each vertical slab, every
    // rectangle spanning the full slab contributes a y-range, and this repo's own
    // IntervalSet<TKey> merges those ranges into disjoint intervals so the
    // covered height sums to exactly the union length - no double-counting where
    // rectangles overlap, and no per-cell scan. O(n^2 log n) against the cell
    // check's O(n^3); the same closed-interval merge MergeIntervals reuses for
    // LC 56.
    public static int TotalAreaBySweepLine(int[][] rectangles)
    {
        var xs = DistinctSorted(rectangles, X1, X2);
        long area = 0;

        for (var i = 0; i < xs.Length - 1; i++)
        {
            area = (area + SlabArea(rectangles, xs[i], xs[i + 1])) % ModularArithmetic.Modulo;
        }

        return (int)area;
    }

    private static long SlabArea(int[][] rectangles, int x1, int x2)
    {
        var yIntervals = new IntervalSet<int>();

        foreach (var rectangle in rectangles)
        {
            if (rectangle[X1] <= x1 && rectangle[X2] >= x2)
            {
                yIntervals.Add(rectangle[Y1], rectangle[Y2]);
            }
        }

        long height = 0;

        for (var j = 0; j < yIntervals.Count; j++)
        {
            var (start, end) = yIntervals.Get(j);
            height += end - start;
        }

        return (x2 - x1) * (height % ModularArithmetic.Modulo);
    }

    private static int[] DistinctSorted(int[][] rectangles, int lowIndex, int highIndex) =>
        rectangles
            .SelectMany(rectangle => new[] { rectangle[lowIndex], rectangle[highIndex] })
            .Distinct()
            .OrderBy(coordinate => coordinate)
            .ToArray();
}
