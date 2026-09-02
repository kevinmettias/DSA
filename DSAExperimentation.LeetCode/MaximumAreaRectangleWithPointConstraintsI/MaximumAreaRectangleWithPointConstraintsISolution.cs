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
                    for (var d = c + 1; d < n; d++)
                    {
                        maxArea = Math.Max(maxArea, RectangleAreaOrNone(points, a, b, c, d));
                    }
                }
            }
        }

        return maxArea;
    }

    private static long RectangleAreaOrNone(int[][] points, int a, int b, int c, int d)
    {
        if (!TryAxisAlignedRectangle(points[a], points[b], points[c], points[d], out var box))
        {
            return None;
        }

        return HasBlockingPoint(points, box) ? None : box.Area;
    }

    // True only when the 4 points are exactly {(minX,minY), (minX,maxY),
    // (maxX,minY), (maxX,maxY)} for some minX < maxX, minY < maxY - i.e. they
    // really are an axis-aligned rectangle's corners, not some other quadrilateral.
    private static bool TryAxisAlignedRectangle(int[] p1, int[] p2, int[] p3, int[] p4, out Box box)
    {
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
                var (x1, y1) = (points[i][0], points[i][1]);
                var (x2, y2) = (points[j][0], points[j][1]);

                if (x1 == x2 || y1 == y2)
                {
                    continue;
                }

                // The rectangle with (x1,y1) and (x2,y2) as opposite corners always has
                // its other two corners at (x1,y2) and (x2,y1) - checked on the RAW
                // coordinates, not min/max-normalized ones, because (i,j) can land on
                // either diagonal of the eventual box. Normalizing first and always
                // checking (minX,maxY)/(maxX,minY) is wrong whenever (i,j) themselves
                // are that anti-diagonal: it would just re-confirm i and j exist.
                if (!corners.Has((x1, y2)) || !corners.Has((x2, y1)))
                {
                    continue;
                }

                var box = new Box(Math.Min(x1, x2), Math.Max(x1, x2), Math.Min(y1, y2), Math.Max(y1, y2));

                if (!HasBlockingPoint(points, box))
                {
                    maxArea = Math.Max(maxArea, box.Area);
                }
            }
        }

        return maxArea;
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

            if (candidate.X >= box.MinX && candidate.X <= box.MaxX &&
                candidate.Y >= box.MinY && candidate.Y <= box.MaxY)
            {
                return true;
            }
        }

        return false;
    }

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
