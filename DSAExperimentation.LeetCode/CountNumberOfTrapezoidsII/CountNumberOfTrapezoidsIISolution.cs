using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.CountNumberOfTrapezoidsII;

// LeetCode 3625. Count Number of Trapezoids II: unlike Part I, the two
// parallel sides need not be horizontal - any pair of sides sharing a slope
// qualifies, so grouping points by y no longer captures the geometry. Both
// strategies answer the same question with the same signature (TwoSumSolution
// precedent).
internal static class CountNumberOfTrapezoidsIISolution
{
    // The C(n, 4) scan of every 4-point combination, checking the three ways
    // to split them into two segments and testing each split with an integer
    // cross product - parallel iff the two direction vectors' cross product
    // is zero, and not degenerate iff the second segment's endpoint isn't
    // also on the first segment's line (otherwise all four points are
    // collinear, not a quadrilateral). No floating-point slope is ever
    // computed. Correct, and the arm the parallel-segment-counting strategy
    // below has to beat; only usable at small n, since C(n, 4) already
    // reaches into the millions past a few dozen points
    // (CountNumberOfTrapezoidsISolution's own baseline note).
    public static int CountTrapezoidsByBruteForce(int[][] points)
    {
        var n = points.Length;
        var count = 0;

        for (var a = 0; a < n; a++)
        {
            for (var b = a + 1; b < n; b++)
            {
                for (var c = b + 1; c < n; c++)
                {
                    for (var d = c + 1; d < n; d++)
                    {
                        if (HasParallelSidePair(points[a], points[b], points[c], points[d]))
                        {
                            count++;
                        }
                    }
                }
            }
        }

        return count;
    }

    private static bool HasParallelSidePair(int[] p, int[] q, int[] r, int[] s) =>
        IsParallelNotCollinear(p, q, r, s) ||
        IsParallelNotCollinear(p, r, q, s) ||
        IsParallelNotCollinear(p, s, q, r);

    private static bool IsParallelNotCollinear(int[] p1, int[] p2, int[] p3, int[] p4)
    {
        var (dx1, dy1) = (p2[0] - p1[0], p2[1] - p1[1]);
        var (dx2, dy2) = (p4[0] - p3[0], p4[1] - p3[1]);

        if (dx1 * dy2 - dy1 * dx2 != 0)
        {
            return false;
        }

        return dx1 * (p3[1] - p1[1]) - dy1 * (p3[0] - p1[0]) != 0;
    }

    // Every side is a segment between two of the points; grouping segments by
    // their reduced (dx, dy) direction (this repo's own HashMap<TKey, TValue>,
    // the TwoSumSolution precedent) turns "count 4-point splits into two
    // disjoint, parallel, non-collinear pairs" into a running frequency count
    // - same slope but a different line - the same "group by an invariant,
    // then combinatorics" move CountNumberOfTrapezoidsISolution's
    // horizontal-pair-counting arm makes for its own (y-only) case. Two
    // segments sharing an endpoint can never land in the same slope-but-
    // different-line bucket - two lines through a common point with the same
    // slope are the same line - so lookupLine's subtraction also throws out
    // every non-disjoint pair for free.
    //
    // A parallelogram's two pairs of opposite sides are both counted this
    // way, so it is tallied twice. Its opposite sides are additionally always
    // equal in length, which an ordinary trapezoid's single qualifying pair
    // need not be; lookupSlopeLength/lookupLineLength repeat the same running
    // count restricted to segment pairs that also share a squared length,
    // isolating exactly the double-counted parallelogram instances, and
    // halving that count removes the duplicate.
    public static int CountTrapezoidsByParallelSegmentCounting(int[][] points)
    {
        var lookupSlope = new HashMap<(int A, int B), int>();
        var lookupLine = new HashMap<(int A, int B, int C), int>();
        var lookupSlopeLength = new HashMap<(int A, int B, int Length), int>();
        var lookupLineLength = new HashMap<(int A, int B, int C, int Length), int>();
        var result = 0L;
        var same = 0L;

        for (var i = 0; i < points.Length; i++)
        {
            var (x1, y1) = (points[i][0], points[i][1]);

            for (var j = 0; j < i; j++)
            {
                var (x2, y2) = (points[j][0], points[j][1]);
                var (dx, dy) = (x2 - x1, y2 - y1);
                var gcd = Gcd(dx, dy);
                var (a, b) = (dx / gcd, dy / gcd);

                if (a < 0 || (a == 0 && b < 0))
                {
                    (a, b) = (-a, -b);
                }

                var c = b * x1 - a * y1;
                var length = dx * dx + dy * dy;

                result += CountThenIncrement(lookupSlope, (a, b)) - CountThenIncrement(lookupLine, (a, b, c));
                same += CountThenIncrement(lookupSlopeLength, (a, b, length)) -
                    CountThenIncrement(lookupLineLength, (a, b, c, length));
            }
        }

        return (int)(result - same / 2);
    }

    private static int CountThenIncrement<TKey>(HashMap<TKey, int> lookup, TKey key)
    {
        lookup.TryGetValue(key, out var count);
        lookup.Set(key, count + 1);

        return count;
    }

    private static int Gcd(int a, int b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }

        return Math.Abs(a);
    }
}
