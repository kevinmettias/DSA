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
    private static long BestFourthArea(Point[] points, (Point P1, Point P2, Point P3) triple, int startIndex)
    {
        var maxArea = None;

        for (var fourth = startIndex; fourth < points.Length; fourth++)
        {
            var quadruple = (triple.P1, triple.P2, triple.P3, points[fourth]);
            var area = RectangleAreaOrNone(points, quadruple);
            maxArea = Math.Max(maxArea, area);
        }

        return maxArea;
    }

    // The four candidate points arrive as one argument, never four: the scan picks them
    // as a corner set, and "are these four a rectangle's corners?" is one question about
    // the set rather than four independent values that could be transposed.
    private static long RectangleAreaOrNone(
        Point[] points, (Point P1, Point P2, Point P3, Point P4) quadruple)
    {
        if (!TryAxisAlignedRectangle(quadruple, out var box))
        {
            return None;
        }

        return HasBlockingPoint(points, box) ? None : box.Area;
    }

    private static bool TryAxisAlignedRectangle(
        (Point P1, Point P2, Point P3, Point P4) quadruple, out Box box)
    {
        var (p1, p2, p3, p4) = quadruple;
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

            if (LiesWithinBox(point, box))
            {
                return true;
            }
        }

        return false;
    }

    // A point blocks the box when it lies inside both of the box's spans; sitting
    // exactly on an edge counts, which is why only the four corners were skipped.
    private static bool LiesWithinBox(Point point, Box box)
        => point.X >= box.MinX && point.X <= box.MaxX && point.Y >= box.MinY && point.Y <= box.MaxY;

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

        var sweep = BuildSweepIndex(sortedPoints);

        var maxArea = None;
        var columnStart = 0;

        while (columnStart < sortedPoints.Length)
        {
            (columnStart, maxArea) = SweepColumn(sortedPoints, sweep, columnStart, maxArea);
        }

        return maxArea;
    }

    // The sweep's three running structures: the compressed-y index, the
    // point-update/range-max tree over it, and the y-intervals still open from an
    // earlier column. All three are reference types the walk mutates in place.
    private static SweepIndex BuildSweepIndex(Point[] sortedPoints)
    {
        var yValues = sortedPoints.Select(p => p.Y).Distinct().OrderBy(y => y).ToArray();
        var yIndex = new Dictionary<int, int>(yValues.Length);

        for (var i = 0; i < yValues.Length; i++)
        {
            yIndex[yValues[i]] = i;
        }

        var lastXAtY = new SegmentTree<int, MaxOperation<int>>(
            Enumerable.Repeat(int.MinValue, yValues.Length).ToArray());
        var openEdges = new Dictionary<(int Y1, int Y2), int>();

        return new SweepIndex(yIndex, lastXAtY, openEdges);
    }

    // One column of the sweep: the y-adjacent pairs inside it are checked against the
    // tree, then this column's own points are written into the tree for later columns.
    // Returns the next column's start, since a pair's x always advances to this one.
    private static (int ColumnStart, long MaxArea) SweepColumn(
        Point[] sortedPoints, SweepIndex sweep, int columnStart, long maxArea)
    {
        var column = ColumnAt(sortedPoints, columnStart);

        maxArea = CheckColumnPairs(sortedPoints, sweep, column, maxArea);

        for (var i = column.Start; i < column.End; i++)
        {
            sweep.LastXAtY.Update(sweep.YIndex[sortedPoints[i].Y], column.X);
        }

        return (column.End, maxArea);
    }

    // One column of the sweep: the run of points that share the x sitting at
    // `columnStart`, named by where it starts, where it ends, and that shared x.
    private static (int Start, int End, int X) ColumnAt(Point[] sortedPoints, int columnStart)
    {
        var columnEnd = columnStart;

        while (columnEnd < sortedPoints.Length && sortedPoints[columnEnd].X == sortedPoints[columnStart].X)
        {
            columnEnd++;
        }

        return (columnStart, columnEnd, sortedPoints[columnStart].X);
    }

    // Every y-adjacent pair of points inside one column. A pair that reaches back to a
    // still-open y-interval wider than that interval's own recorded x re-forms a
    // rectangle; either way this column becomes the interval's new x.
    private static long CheckColumnPairs(
        Point[] sortedPoints,
        SweepIndex sweep,
        (int Start, int End, int X) column,
        long maxArea)
    {
        for (var i = column.Start; i < column.End - 1; i++)
        {
            var y1 = sortedPoints[i].Y;
            var y2 = sortedPoints[i + 1].Y;
            var key = (y1, y2);
            var mostRecentXInRange = sweep.LastXAtY.Query(sweep.YIndex[y1], sweep.YIndex[y2]);

            if (sweep.OpenEdges.TryGetValue(key, out var xLeft) && mostRecentXInRange <= xLeft)
            {
                maxArea = Math.Max(maxArea, (long)(column.X - xLeft) * (y2 - y1));
            }

            sweep.OpenEdges[key] = column.X;
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

    private readonly record struct SweepIndex(
        Dictionary<int, int> YIndex,
        SegmentTree<int, MaxOperation<int>> LastXAtY,
        Dictionary<(int Y1, int Y2), int> OpenEdges);

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
