using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.CountLatticePointsInsideACircle;

// LeetCode 2249. Count Lattice Points Inside a Circle: circles[i] is
// [x, y, r], and the answer is how many integer points lie inside or on at least
// one of them - so the whole problem is the SIZE OF A UNION, and the two strategies
// differ only in how they avoid counting a shared point twice.
//
// The baseline never needs a set at all: it walks the combined bounding box of
// every circle and asks, once per point, whether any circle covers it -
// O(totalArea * circleCount). The composed answer instead scans each circle's own
// local bounding box and lets this repo's Set<Element> (HashMap<Element,bool>-backed)
// collapse the overlaps - O(sum of each circle's own area), which wins whenever the
// circles are scattered far more widely than their radii.
internal static class CountLatticePointsInsideACircleSolution
{
    private const int CenterXIndex = 0;
    private const int CenterYIndex = 1;
    private const int RadiusIndex = 2;

    // The textbook answer, deliberately written without this repo's primitives: one
    // counter and two loops over the whole combined box, re-testing every circle at
    // every point.
    public static int CountLatticePointsByFullGridScan(int[][] circles)
    {
        var box = BoundingBoxOf(circles);
        var count = 0;

        for (var x = box.MinX; x <= box.MaxX; x++)
        {
            for (var y = box.MinY; y <= box.MaxY; y++)
            {
                if (IsInsideAnyCircle(circles, x, y))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static BoundingBox BoundingBoxOf(int[][] circles)
    {
        var box = new BoundingBox(int.MaxValue, int.MinValue, int.MaxValue, int.MinValue);

        foreach (var circle in circles)
        {
            var radius = circle[RadiusIndex];

            box = new BoundingBox(
                Math.Min(box.MinX, circle[CenterXIndex] - radius),
                Math.Max(box.MaxX, circle[CenterXIndex] + radius),
                Math.Min(box.MinY, circle[CenterYIndex] - radius),
                Math.Max(box.MaxY, circle[CenterYIndex] + radius));
        }

        return box;
    }

    private static bool IsInsideAnyCircle(int[][] circles, int x, int y)
    {
        foreach (var circle in circles)
        {
            if (Covers(circle, x - circle[CenterXIndex], y - circle[CenterYIndex]))
            {
                return true;
            }
        }

        return false;
    }

    // Membership is the whole job, so Set<Element> is the whole answer: each circle
    // contributes only the points in its own bounding box, and TryAdd silently drops
    // the ones an earlier circle already claimed.
    public static int CountLatticePointsByPerCircleSetUnion(int[][] circles)
    {
        var points = new Set<(int X, int Y)>();

        foreach (var circle in circles)
        {
            AddCirclePoints(circle, points);
        }

        return points.Count;
    }

    private static void AddCirclePoints(int[] circle, Set<(int X, int Y)> points)
    {
        var x = circle[CenterXIndex];
        var y = circle[CenterYIndex];
        var radius = circle[RadiusIndex];

        for (var dx = -radius; dx <= radius; dx++)
        {
            for (var dy = -radius; dy <= radius; dy++)
            {
                if (Covers(circle, dx, dy))
                {
                    points.TryAdd((x + dx, y + dy));
                }
            }
        }
    }

    // Inclusive: LeetCode counts a point exactly on the circumference as inside.
    private static bool Covers(int[] circle, int dx, int dy)
        => (dx * dx) + (dy * dy) <= circle[RadiusIndex] * circle[RadiusIndex];

    // The smallest axis-aligned box containing every circle. Empty when there are
    // no circles at all, which leaves the scan's loops with nothing to visit.
    private readonly record struct BoundingBox(int MinX, int MaxX, int MinY, int MaxY);
}
