using DSAExperimentation.Algorithms.Geometry;

namespace DSAExperimentation.LeetCode.ErectTheFence;

// LeetCode 587. Erect the Fence: the smallest set of trees a fence touching every
// tree must pass through - i.e. every point on the convex hull's boundary,
// corners AND every point collinear-and-between on an edge, not just the strict
// corners a textbook hull returns.
//
// Both strategies return that same point set. They differ only in how they find
// the hull's candidate edges: the baseline tests every ordered pair of points as
// a candidate hull line, the composed strategy takes the O(h) strict-corner edges
// from Algorithms.Geometry.ConvexHull - Andrew's monotone chain over MergeSort. Both
// then run the identical collinearity/betweenness scan to fold in the boundary
// points a strict hull would drop.
//
// The benchmark this migrated from had both arms return fence.Count rather than
// the fence itself, to keep the measured method's return value cheap - a
// measurement choice, not part of either algorithm's answer. LeetCode's actual
// answer is the point set, so both are promoted to return it here.
internal static class ErectTheFenceSolution
{
    private const int MinPointsForTurn = 2;

    // The O(n^3) baseline: for every ordered pair of points, check whether every
    // other point lies on one side of the line through them (or on it) - if so
    // it is a hull edge, and every point collinear-and-between on it belongs in
    // the fence. Written without this repo's primitives - the arm the composed
    // solution below has to beat.
    public static List<(int X, int Y)> OuterTreesByBruteForceHalfPlaneScan((int X, int Y)[] points)
    {
        if (points.Length < MinPointsForTurn + 1)
        {
            return points.ToList();
        }

        var fence = new HashSet<(int X, int Y)>();

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = 0; j < points.Length; j++)
            {
                AddHullLinePoints(fence, points, i, j);
            }
        }

        return fence.ToList();
    }

    // The ordered pair (firstIndex, secondIndex) is a candidate hull edge exactly when
    // every other point lies on one side of the line through it (or on the line itself).
    private static void AddHullLinePoints(
        HashSet<(int X, int Y)> fence, (int X, int Y)[] points, int firstIndex, int secondIndex)
    {
        if (firstIndex == secondIndex)
        {
            return;
        }

        var a = points[firstIndex];
        var b = points[secondIndex];

        if (IsHullLine(points, a, b))
        {
            AddSegmentPoints(fence, points, a, b);
        }
    }

    private static bool IsHullLine(
        (int X, int Y)[] points, (int X, int Y) lineStart, (int X, int Y) lineEnd)
    {
        var side = 0;

        foreach (var c in points)
        {
            if (!TryUpdateSide(lineStart, lineEnd, c, ref side))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryUpdateSide(
        (int X, int Y) lineStart, (int X, int Y) lineEnd, (int X, int Y) testPoint, ref int side)
    {
        var cross = Cross(lineStart, lineEnd, testPoint);

        if (cross == 0)
        {
            return true;
        }

        var thisSide = cross > 0 ? 1 : -1;

        if (side == 0)
        {
            side = thisSide;
        }
        else if (side != thisSide)
        {
            return false;
        }

        return true;
    }

    // ConvexHull's strict corners, counter-clockwise, then the same
    // collinearity/betweenness scan restricted to just those h edges instead of
    // every O(n^2) pair.
    public static List<(int X, int Y)> OuterTreesByMonotoneChain((int X, int Y)[] points)
    {
        if (points.Length < MinPointsForTurn + 1)
        {
            return points.ToList();
        }

        var corners = ConvexHull.Corners(points);
        var fence = new HashSet<(int X, int Y)>();

        for (var i = 0; i < corners.Length; i++)
        {
            var a = corners[i];
            var b = corners[(i + 1) % corners.Length];

            AddSegmentPoints(fence, points, a, b);
        }

        return fence.ToList();
    }

    private static void AddSegmentPoints(
        HashSet<(int X, int Y)> fence,
        (int X, int Y)[] points,
        (int X, int Y) lineStart,
        (int X, int Y) lineEnd)
    {
        foreach (var p in points)
        {
            if (IsOnSegment(lineStart, lineEnd, p))
            {
                fence.Add(p);
            }
        }
    }

    private static bool IsOnSegment(
        (int X, int Y) lineStart, (int X, int Y) lineEnd, (int X, int Y) point)
        => Cross(lineStart, lineEnd, point) == 0
            && point.X >= Math.Min(lineStart.X, lineEnd.X)
            && point.X <= Math.Max(lineStart.X, lineEnd.X)
            && point.Y >= Math.Min(lineStart.Y, lineEnd.Y)
            && point.Y <= Math.Max(lineStart.Y, lineEnd.Y);

    private static long Cross(
        (int X, int Y) origin, (int X, int Y) firstPoint, (int X, int Y) secondPoint)
        => (long)(firstPoint.X - origin.X) * (secondPoint.Y - origin.Y)
            - (long)(firstPoint.Y - origin.Y) * (secondPoint.X - origin.X);
}
