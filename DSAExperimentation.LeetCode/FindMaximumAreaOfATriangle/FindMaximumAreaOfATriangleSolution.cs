using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.FindMaximumAreaOfATriangle;

// LeetCode 3588. Find Maximum Area of a Triangle: given n points on an infinite
// plane, find twice the maximum area of a triangle with at least one side
// parallel to the x-axis or the y-axis, formed by any three of the points, or
// -1 if no such triangle exists.
//
// The key geometric fact both strategies lean on: for a triangle with one side
// lying on a horizontal (or vertical) line, its area is exactly half that
// side's length times the perpendicular distance from the third point to that
// line - independent of where along the line the base sits, and independent of
// where the third point falls on the other axis. That collapses "pick any 3
// points" down to "pick a row or column, then the single farthest point from
// it anywhere in the whole set."
internal static class FindMaximumAreaOfATriangleSolution
{
    // Textbook O(n^3): every triple, keep only those with a shared x or shared y
    // between some pair, score the rest with the general shoelace formula (which
    // agrees with base*height/2 for these triangles regardless of which side
    // qualifies) - the arm the row/column strategy below has to beat.
    public static long MaxAreaByBruteForce(int[][] coords)
    {
        long best = LeetCodeAnswer.None;

        for (var i = 0; i < coords.Length; i++)
        {
            for (var j = i + 1; j < coords.Length; j++)
            {
                for (var k = j + 1; k < coords.Length; k++)
                {
                    best = BestOfTriple(best, coords[i], coords[j], coords[k]);
                }
            }
        }

        return best;
    }

    // The running best after considering one triple: a triple with no axis-parallel
    // side, or one that encloses no area at all, leaves the answer where it was.
    private static long BestOfTriple(long best, int[] a, int[] b, int[] c)
    {
        if (!HasAxisParallelSide(a, b, c))
        {
            return best;
        }

        var doubledArea = DoubledArea(a, b, c);

        if (doubledArea > 0 && doubledArea > best)
        {
            return doubledArea;
        }

        return best;
    }

    private static bool HasAxisParallelSide(int[] a, int[] b, int[] c) =>
        SharesAnAxis(a, b) || SharesAnAxis(b, c) || SharesAnAxis(a, c);

    private static bool SharesAnAxis(int[] p, int[] q) => p[0] == q[0] || p[1] == q[1];

    private static long DoubledArea(int[] a, int[] b, int[] c) =>
        Math.Abs(((long)(b[0] - a[0]) * (c[1] - a[1])) - ((long)(c[0] - a[0]) * (b[1] - a[1])));

    // Composed: one O(n) pass over this repo's own HashMap<TKey, TValue>, grouping
    // points into a running [min, max] x-span per row (shared y) and [min, max]
    // y-span per column (shared x), alongside a single global bounding box. Every
    // row with more than one point is a candidate horizontal base; its best
    // triangle pairs that base's x-span with the farthest point from the row's y
    // anywhere in the whole point set (an O(1) lookup against the global y bounds,
    // per the class-level geometric fact above) - symmetrically for columns. No
    // triple is ever materialized.
    public static long MaxAreaBySpreadHashMap(int[][] coords)
    {
        var rowSpans = new HashMap<int, AxisSpan>();
        var columnSpans = new HashMap<int, AxisSpan>();
        var bounds = (MinX: int.MaxValue, MaxX: int.MinValue, MinY: int.MaxValue, MaxY: int.MinValue);

        foreach (var point in coords)
        {
            bounds = AbsorbPoint(point, rowSpans, columnSpans, bounds);
        }

        long best = LeetCodeAnswer.None;
        best = BestAlongAxis(rowSpans, (bounds.MinY, bounds.MaxY), best);
        best = BestAlongAxis(columnSpans, (bounds.MinX, bounds.MaxX), best);

        return best;
    }

    // Fold one point into the spans and the global bounding box, returning the box -
    // the per-point step of the single pass above.
    private static (int MinX, int MaxX, int MinY, int MaxY) AbsorbPoint(
        int[] point,
        HashMap<int, AxisSpan> rowSpans,
        HashMap<int, AxisSpan> columnSpans,
        (int MinX, int MaxX, int MinY, int MaxY) bounds)
    {
        var (x, y) = (point[0], point[1]);

        rowSpans.Set(y, rowSpans.TryGetValue(y, out var row)
            ? WidenedSpan(row, x)
            : SinglePointSpan(x));

        columnSpans.Set(x, columnSpans.TryGetValue(x, out var column)
            ? WidenedSpan(column, y)
            : SinglePointSpan(y));

        return ExpandBounds(bounds, x, y);
    }

    private static AxisSpan WidenedSpan(AxisSpan span, int value) =>
        new(Math.Min(span.Min, value), Math.Max(span.Max, value));

    private static AxisSpan SinglePointSpan(int value) => new(value, value);

    // The bounding box grown to include one more point.
    private static (int MinX, int MaxX, int MinY, int MaxY) ExpandBounds(
        (int MinX, int MaxX, int MinY, int MaxY) bounds, int x, int y) =>
        (
            Math.Min(bounds.MinX, x),
            Math.Max(bounds.MaxX, x),
            Math.Min(bounds.MinY, y),
            Math.Max(bounds.MaxY, y));

    // The best triangle over one axis of spans, folding each candidate base into the
    // answer. A base is a line whose span is non-empty; its height is the farthest
    // point from that line, measured against the bounding box on the other axis.
    private static long BestAlongAxis(
        HashMap<int, AxisSpan> spans, (int Near, int Far) reachBounds, long best)
    {
        foreach (var line in spans.Keys)
        {
            spans.TryGetValue(line, out var span);
            var reach = Math.Max(line - reachBounds.Near, reachBounds.Far - line);

            best = BestOfSpan(best, reach, span);
        }

        return best;
    }

    // The running best after considering one span as a candidate base: a span of no
    // width is not a base, and a candidate that encloses no area or does not beat the
    // answer leaves it alone.
    private static long BestOfSpan(long best, long reach, AxisSpan span)
    {
        var extent = span.Max - span.Min;

        if (extent == 0)
        {
            return best;
        }

        var candidate = extent * reach;

        if (candidate > 0 && candidate > best)
        {
            return candidate;
        }

        return best;
    }
}
