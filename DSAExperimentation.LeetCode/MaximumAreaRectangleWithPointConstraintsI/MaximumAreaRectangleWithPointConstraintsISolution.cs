using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MaximumAreaRectangleWithPointConstraintsI;

// LeetCode 3380. Maximum Area Rectangle With Point Constraints I: among the given
// points, find the largest axis-aligned rectangle whose four corners are all
// points in the input and which contains no other given point inside it or on
// its border. Return -1 if no such rectangle exists. (Part I's n is small enough
// that both strategies below stay polynomial in n; Part II, with n up to 1e5,
// would need a genuinely different approach and is out of scope here.)
internal static class MaximumAreaRectangleWithPointConstraintsISolution
{
    private const long None = -1;

    // Baseline: every combination of 4 of the n points, checked for being exactly
    // an axis-aligned rectangle's corner set, then checked against every other
    // point for a border/interior violation. O(n^4 * n) = O(n^5) - the arm the
    // corner-lookup strategy has to beat, appropriate only because Part I's n is
    // small.
    public static long MaxAreaByQuadrupleScan(int[][] points)
    {
        var n = points.Length;
        var maxArea = None;

        for (var a = 0; a < n; a++)
        {
            for (var b = a + 1; b < n; b++)
            {
                for (var c = b + 1; c < n; c++)
                {
                    var triple = (points[a], points[b], points[c]);
                    var area = BestFourthArea(points, triple, c + 1);
                    maxArea = Math.Max(maxArea, area);
                }
            }
        }

        return maxArea;
    }

    // The innermost of the scan's four nested loops, lifted out so the three that
    // pick a, b and c read as a flat sequence: the best area this triple of corners
    // can complete with any later fourth point.
    private static long BestFourthArea(int[][] points, (int[] P1, int[] P2, int[] P3) triple, int startIndex)
    {
        var best = None;

        for (var d = startIndex; d < points.Length; d++)
        {
            var quadruple = (triple.P1, triple.P2, triple.P3, points[d]);
            var area = RectangleAreaOrNone(points, quadruple);
            best = Math.Max(best, area);
        }

        return best;
    }

    // The four candidate points arrive as one argument, never four: the scan picks them
    // as a corner set, and "are these four a rectangle's corners?" is one question about
    // the set rather than four independent values that could be transposed.
    private static long RectangleAreaOrNone(
        int[][] points, (int[] P1, int[] P2, int[] P3, int[] P4) quadruple)
    {
        if (!TryAxisAlignedRectangle(quadruple, out var box))
        {
            return None;
        }

        return HasBlockingPoint(points, box) ? None : box.Area;
    }

    // True only when the 4 points are exactly {(minX,minY), (minX,maxY),
    // (maxX,minY), (maxX,maxY)} for some minX < maxX, minY < maxY - i.e. they
    // really are an axis-aligned rectangle's corners, not some other quadrilateral.
    private static bool TryAxisAlignedRectangle(
        (int[] P1, int[] P2, int[] P3, int[] P4) quadruple, out Box box)
    {
        var (p1, p2, p3, p4) = quadruple;
        var xs = new[] { p1[0], p2[0], p3[0], p4[0] }.Distinct().OrderBy(x => x).ToArray();
        var ys = new[] { p1[1], p2[1], p3[1], p4[1] }.Distinct().OrderBy(y => y).ToArray();

        box = default;

        if (xs.Length != 2 || ys.Length != 2)
        {
            return false;
        }

        box = new Box(xs[0], xs[1], ys[0], ys[1]);
        var corners = new HashSet<(int X, int Y)> { (p1[0], p1[1]), (p2[0], p2[1]), (p3[0], p3[1]), (p4[0], p4[1]) };

        return corners.SetEquals(box.Corners());
    }

    // Composed: only the O(n^2) pairs of points that could be a rectangle's
    // diagonal are considered, using this repo's own Set<T> for O(1) "does the
    // opposite corner exist" membership checks - the same "Set<T>, not IEnumerable"
    // shape OpenTheLockSolution's deadend lookup uses, so it can never collide
    // with the LeetCode-shaped int[][] overload. O(n^2) diagonals * O(n) border
    // check = O(n^3).
    public static long MaxAreaByCornerLookup(int[][] points)
    {
        var corners = new Set<(int X, int Y)>(points.Select(p => (p[0], p[1])));

        return MaxAreaByCornerLookup(points, corners);
    }

    public static long MaxAreaByCornerLookup(int[][] points, Set<(int X, int Y)> corners)
    {
        var n = points.Length;
        var maxArea = None;

        for (var i = 0; i < n; i++)
        {
            for (var j = i + 1; j < n; j++)
            {
                var area = DiagonalAreaOrNone(points, corners, i, j);
                maxArea = Math.Max(maxArea, area);
            }
        }

        return maxArea;
    }

    // The area of the rectangle that diagonal (i, j) completes, or None when it is not
    // a usable rectangle: a diagonal has to span both axes, both remaining corners have
    // to be given points, and no other point may sit inside the box or on its border.
    private static long DiagonalAreaOrNone(int[][] points, Set<(int X, int Y)> corners, int i, int j)
    {
        var (x1, y1) = (points[i][0], points[i][1]);
        var (x2, y2) = (points[j][0], points[j][1]);

        if (x1 == x2 || y1 == y2)
        {
            return None;
        }

        // The rectangle with (x1,y1) and (x2,y2) as opposite corners always has its
        // other two corners at (x1,y2) and (x2,y1) - checked on the RAW coordinates,
        // not min/max-normalized ones, because (i,j) can land on either diagonal of
        // the eventual box. Normalizing first and always checking
        // (minX,maxY)/(maxX,minY) is wrong whenever (i,j) themselves are that
        // anti-diagonal: it would just re-confirm i and j exist.
        if (!corners.Has((x1, y2)) || !corners.Has((x2, y1)))
        {
            return None;
        }

        var box = new Box(Math.Min(x1, x2), Math.Max(x1, x2), Math.Min(y1, y2), Math.Max(y1, y2));
        return HasBlockingPoint(points, box) ? None : box.Area;
    }

    // Any point other than the box's own 4 corners that lies inside the box or on
    // its border invalidates it - LeetCode's own rule, checked here once for both
    // strategies since it is pure geometry, not part of either search.
    private static bool HasBlockingPoint(int[][] points, Box box)
    {
        foreach (var point in points)
        {
            var candidate = (X: point[0], Y: point[1]);

            if (box.IsCorner(candidate))
            {
                continue;
            }

            if (IsWithinBox(box, candidate))
            {
                return true;
            }
        }

        return false;
    }

    // The candidate lies within the box's extent, border included.
    private static bool IsWithinBox(Box box, (int X, int Y) point)
        => point.X >= box.MinX && point.X <= box.MaxX
            && point.Y >= box.MinY && point.Y <= box.MaxY;

    private readonly record struct Box(int MinX, int MaxX, int MinY, int MaxY)
    {
        public long Area => (long)(MaxX - MinX) * (MaxY - MinY);

        public IEnumerable<(int X, int Y)> Corners()
        {
            yield return (MinX, MinY);
            yield return (MinX, MaxY);
            yield return (MaxX, MinY);
            yield return (MaxX, MaxY);
        }

        public bool IsCorner((int X, int Y) point) =>
            (point.X == MinX || point.X == MaxX) && (point.Y == MinY || point.Y == MaxY);
    }
}
