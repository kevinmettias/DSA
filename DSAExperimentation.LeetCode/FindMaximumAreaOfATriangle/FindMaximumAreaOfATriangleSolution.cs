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
                    if (!HasAxisParallelSide(coords[i], coords[j], coords[k]))
                    {
                        continue;
                    }

                    var doubledArea = DoubledArea(coords[i], coords[j], coords[k]);

                    if (doubledArea > 0 && doubledArea > best)
                    {
                        best = doubledArea;
                    }
                }
            }
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
        var rowSpans = new HashMap<int, (int MinX, int MaxX)>();
        var columnSpans = new HashMap<int, (int MinY, int MaxY)>();
        var minX = int.MaxValue;
        var maxX = int.MinValue;
        var minY = int.MaxValue;
        var maxY = int.MinValue;

        foreach (var point in coords)
        {
            var (x, y) = (point[0], point[1]);
            minX = Math.Min(minX, x);
            maxX = Math.Max(maxX, x);
            minY = Math.Min(minY, y);
            maxY = Math.Max(maxY, y);

            rowSpans.Set(y, rowSpans.TryGetValue(y, out var row)
                ? (Math.Min(row.MinX, x), Math.Max(row.MaxX, x))
                : (x, x));

            columnSpans.Set(x, columnSpans.TryGetValue(x, out var column)
                ? (Math.Min(column.MinY, y), Math.Max(column.MaxY, y))
                : (y, y));
        }

        long best = LeetCodeAnswer.None;

        foreach (var y in rowSpans.Keys)
        {
            rowSpans.TryGetValue(y, out var row);
            var width = row.MaxX - row.MinX;

            if (width == 0)
            {
                continue;
            }

            var reach = Math.Max(y - minY, maxY - y);
            var candidate = (long)width * reach;

            if (candidate > 0 && candidate > best)
            {
                best = candidate;
            }
        }

        foreach (var x in columnSpans.Keys)
        {
            columnSpans.TryGetValue(x, out var column);
            var height = column.MaxY - column.MinY;

            if (height == 0)
            {
                continue;
            }

            var reach = Math.Max(x - minX, maxX - x);
            var candidate = (long)height * reach;

            if (candidate > 0 && candidate > best)
            {
                best = candidate;
            }
        }

        return best;
    }
}
