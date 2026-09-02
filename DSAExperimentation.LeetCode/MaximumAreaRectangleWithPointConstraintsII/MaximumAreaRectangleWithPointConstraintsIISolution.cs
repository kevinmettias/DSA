using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.MaximumAreaRectangleWithPointConstraintsII;

// LeetCode 3382. Maximum Area Rectangle With Point Constraints II: the same rule
// as Part I (MaximumAreaRectangleWithPointConstraintsI) - among the given points,
// find the largest axis-aligned rectangle whose four corners are all points in
// the input and which contains no other given point inside it or on its border,
// or -1 if none exists - but with n up to 2e5, where Part I's polynomial
// strategies are no longer tractable. That is the "genuinely different approach"
// Part I's own doc comment flagged as out of scope for it: a left-to-right sweep
// over x-columns, using this repo's own SegmentTree<Element, MaxOperation<Element>>
// as a point-update/range-max index over compressed y so each column does
// O(log n) work instead of an O(n) border scan.
internal static class MaximumAreaRectangleWithPointConstraintsIISolution
{
    private const long None = -1;

    public readonly record struct Point(int X, int Y);

    // Textbook baseline: every combination of 4 of the n points, checked for
    // being exactly an axis-aligned rectangle's corner set, then checked against
    // every other point for a border/interior violation - O(n^5), the same shape
    // Part I's own MaxAreaByQuadrupleScan uses. Deliberately independent of Part
    // I's class (a private baseline, not a shared production primitive) and only
    // tractable at the tiny n LeetCode's published examples use.
    public static long MaxAreaByQuadrupleScan(int[] xCoord, int[] yCoord)
    {
        var points = BuildPoints(xCoord, yCoord);
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

    private static long RectangleAreaOrNone(Point[] points, int a, int b, int c, int d)
    {
        if (!TryAxisAlignedRectangle(points[a], points[b], points[c], points[d], out var box))
        {
            return None;
        }

        return HasBlockingPoint(points, box) ? None : box.Area;
    }

    private static bool TryAxisAlignedRectangle(Point p1, Point p2, Point p3, Point p4, out Box box)
    {
        var xs = new[] { p1.X, p2.X, p3.X, p4.X }.Distinct().OrderBy(x => x).ToArray();
        var ys = new[] { p1.Y, p2.Y, p3.Y, p4.Y }.Distinct().OrderBy(y => y).ToArray();

        box = default;

        if (xs.Length != 2 || ys.Length != 2)
        {
            return false;
        }

        box = new Box(xs[0], xs[1], ys[0], ys[1]);
        var corners = new HashSet<Point> { p1, p2, p3, p4 };

        return corners.SetEquals(box.Corners());
    }

    private static bool HasBlockingPoint(Point[] points, Box box)
    {
        foreach (var point in points)
        {
            if (box.IsCorner(point))
            {
                continue;
            }

            if (point.X >= box.MinX && point.X <= box.MaxX && point.Y >= box.MinY && point.Y <= box.MaxY)
            {
                return true;
            }
        }

        return false;
    }

    // Composed: sweep columns (distinct x, ascending) left to right. Within a
    // column, only y-adjacent pairs of points can ever be a rectangle's vertical
    // edge - any other point at that column between them would sit on the edge
    // itself. For an adjacent pair (y1, y2), the segment tree's Query(y1, y2)
    // (compressed indices, closed range) reports the most recent x at which
    // *any* point occupied a y in [y1, y2] - including the pair's own earlier
    // occurrence at xLeft, which is why the check is a *strict* ">": a hit that
    // is exactly xLeft is the pair re-confirming itself, not an intruder.
    // Whenever this column re-forms an already-open pair, xLeft always advances
    // to this column afterward - whether or not the rectangle (xLeft, x, y1, y2)
    // was itself valid - because this column's own two points sit exactly on
    // that pair's y1/y2 border, which invalidates any *wider* rectangle using
    // the same pair that would have to cross over this column. Every point
    // updates the tree only after its own column's pairs are checked, so a
    // column's two edge-forming points never block each other or themselves.
    // O(n log n) total: one query and, per point, one update.
    public static long MaxAreaBySweepSegmentTree(int[] xCoord, int[] yCoord)
    {
        var points = BuildPoints(xCoord, yCoord);
        Array.Sort(points, (p, q) => p.X != q.X ? p.X.CompareTo(q.X) : p.Y.CompareTo(q.Y));

        return MaxAreaBySweepSegmentTree(points);
    }

    public static long MaxAreaBySweepSegmentTree(Point[] sortedPoints)
    {
        if (sortedPoints.Length < 4)
        {
            return None;
        }

        var yValues = sortedPoints.Select(p => p.Y).Distinct().OrderBy(y => y).ToArray();
        var yIndex = new Dictionary<int, int>(yValues.Length);

        for (var i = 0; i < yValues.Length; i++)
        {
            yIndex[yValues[i]] = i;
        }

        var lastXAtY = new SegmentTree<int, MaxOperation<int>>(
            Enumerable.Repeat(int.MinValue, yValues.Length).ToArray());
        var openEdges = new Dictionary<(int Y1, int Y2), int>();
        var maxArea = None;

        var columnStart = 0;

        while (columnStart < sortedPoints.Length)
        {
            var columnEnd = columnStart;

            while (columnEnd < sortedPoints.Length && sortedPoints[columnEnd].X == sortedPoints[columnStart].X)
            {
                columnEnd++;
            }

            var x = sortedPoints[columnStart].X;

            for (var i = columnStart; i < columnEnd - 1; i++)
            {
                var y1 = sortedPoints[i].Y;
                var y2 = sortedPoints[i + 1].Y;
                var key = (y1, y2);
                var mostRecentXInRange = lastXAtY.Query(yIndex[y1], yIndex[y2]);

                if (openEdges.TryGetValue(key, out var xLeft) && mostRecentXInRange <= xLeft)
                {
                    maxArea = Math.Max(maxArea, (long)(x - xLeft) * (y2 - y1));
                }

                openEdges[key] = x;
            }

            for (var i = columnStart; i < columnEnd; i++)
            {
                lastXAtY.Update(yIndex[sortedPoints[i].Y], x);
            }

            columnStart = columnEnd;
        }

        return maxArea;
    }

    private static Point[] BuildPoints(int[] xCoord, int[] yCoord)
    {
        var points = new Point[xCoord.Length];

        for (var i = 0; i < xCoord.Length; i++)
        {
            points[i] = new Point(xCoord[i], yCoord[i]);
        }

        return points;
    }

    private readonly record struct Box(int MinX, int MaxX, int MinY, int MaxY)
    {
        public long Area => (long)(MaxX - MinX) * (MaxY - MinY);

        public IEnumerable<Point> Corners()
        {
            yield return new Point(MinX, MinY);
            yield return new Point(MinX, MaxY);
            yield return new Point(MaxX, MinY);
            yield return new Point(MaxX, MaxY);
        }

        public bool IsCorner(Point point) =>
            (point.X == MinX || point.X == MaxX) && (point.Y == MinY || point.Y == MaxY);
    }
}
