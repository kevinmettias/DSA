using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MinimumAreaRectangle;

// LeetCode 939. Minimum Area Rectangle: the smallest area of an axis-aligned
// rectangle whose four corners are all in the given point set, or 0 if none exists.
//
// Both strategies try every pair of points as a candidate diagonal (x1,y1)-(x2,y2):
// such a rectangle exists iff the other two corners (x1,y2) and (x2,y1) are also
// present. They differ only in how that presence is confirmed.
//
// MinAreaRectByLinearScan is the naive baseline - it rescans the raw points array
// for each corner (O(n) per check, O(n^3) overall), written with nothing but BCL
// arrays.
//
// MinAreaRectBySetLookup builds this repo's own Set<(int X, int Y)> once up front
// (the same membership primitive PerfectRectangleSolution uses for corner
// bookkeeping) and confirms each corner in O(1), the same "swap a linear rescan for
// a hash lookup" move TwoSumSolution makes for its own pair.
internal static class MinimumAreaRectangleSolution
{
    private const int NoRectangle = 0;

    public static int MinAreaRectByLinearScan(int[][] points)
    {
        var minArea = int.MaxValue;

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                var area = ScannedRectangleArea(points, points[i], points[j]);

                if (area is not null)
                {
                    minArea = Math.Min(minArea, area.Value);
                }
            }
        }

        return minArea == int.MaxValue ? NoRectangle : minArea;
    }

    private static int? ScannedRectangleArea(int[][] points, int[] corner, int[] opposite)
    {
        var (x1, y1) = (corner[0], corner[1]);
        var (x2, y2) = (opposite[0], opposite[1]);

        if (x1 == x2 || y1 == y2)
        {
            return null;
        }

        if (HasPoint(points, x1, y2) && HasPoint(points, x2, y1))
        {
            return Math.Abs((x2 - x1) * (y2 - y1));
        }

        return null;
    }

    private static bool HasPoint(int[][] points, int pointX, int pointY)
    {
        foreach (var point in points)
        {
            if (point[0] == pointX && point[1] == pointY)
            {
                return true;
            }
        }

        return false;
    }

    public static int MinAreaRectBySetLookup(int[][] points)
    {
        var seen = BuildSeenSet(points);
        var minArea = int.MaxValue;

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                var area = SetRectangleArea(points[i], points[j], seen);

                if (area is not null)
                {
                    minArea = Math.Min(minArea, area.Value);
                }
            }
        }

        return minArea == int.MaxValue ? NoRectangle : minArea;
    }

    private static Set<(int X, int Y)> BuildSeenSet(int[][] points)
    {
        var seen = new Set<(int X, int Y)>();

        foreach (var point in points)
        {
            seen.TryAdd((point[0], point[1]));
        }

        return seen;
    }

    private static int? SetRectangleArea(int[] corner, int[] opposite, Set<(int X, int Y)> seen)
    {
        var (x1, y1) = (corner[0], corner[1]);
        var (x2, y2) = (opposite[0], opposite[1]);

        if (x1 == x2 || y1 == y2)
        {
            return null;
        }

        if (seen.Has((x1, y2)) && seen.Has((x2, y1)))
        {
            return Math.Abs((x2 - x1) * (y2 - y1));
        }

        return null;
    }
}
