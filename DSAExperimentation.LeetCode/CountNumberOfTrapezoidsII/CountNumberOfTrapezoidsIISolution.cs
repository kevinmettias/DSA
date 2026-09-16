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
                    count += CountTrapezoidsWithFirstThree(points, a, b, c);
                }
            }
        }

        return count;
    }

    // The fourth point (the index after the third) of a 4-point combination: the split
    // is a trapezoid when one of its three pairings gives two parallel, non-collinear
    // sides. Lifted out of the brute-force scan above, which is then only three loops
    // deep.
    private static int CountTrapezoidsWithFirstThree(
        int[][] points, int firstIndex, int secondIndex, int thirdIndex)
    {
        var count = 0;

        for (var fourthIndex = thirdIndex + 1; fourthIndex < points.Length; fourthIndex++)
        {
            if (HasParallelSidePair(
                points[firstIndex], points[secondIndex], points[thirdIndex], points[fourthIndex]))
            {
                count++;
            }
        }

        return count;
    }

    private static bool HasParallelSidePair(
        int[] firstPoint, int[] secondPoint, int[] thirdPoint, int[] fourthPoint) =>
        IsParallelNotCollinear(firstPoint, secondPoint, thirdPoint, fourthPoint) ||
        IsParallelNotCollinear(firstPoint, thirdPoint, secondPoint, fourthPoint) ||
        IsParallelNotCollinear(firstPoint, fourthPoint, secondPoint, thirdPoint);

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
        var lookups = (
            LookupSlope: new HashMap<(int A, int B), int>(),
            LookupLine: new HashMap<(int A, int B, int C), int>(),
            LookupSlopeLength: new HashMap<(int A, int B, int Length), int>(),
            LookupLineLength: new HashMap<(int A, int B, int C, int Length), int>());

        var result = 0L;
        var same = 0L;

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = 0; j < i; j++)
            {
                var (counted, duplicate) = CountPair(points, i, j, lookups);
                result += counted;
                same += duplicate;
            }
        }

        return (int)(result - same / 2);
    }

    // One ordered pair of points, firstIndex above secondIndex: canonicalize the
    // segment's direction and length into the four lookup keys, then report the two
    // running counts this pair moves - `Counted` for "same slope, a different line",
    // `Duplicate` for the parallelogram tally that is halved once the scan ends.
    private static (long Counted, long Duplicate) CountPair(
        int[][] points,
        int firstIndex,
        int secondIndex,
        (HashMap<(int A, int B), int> LookupSlope,
            HashMap<(int A, int B, int C), int> LookupLine,
            HashMap<(int A, int B, int Length), int> LookupSlopeLength,
            HashMap<(int A, int B, int C, int Length), int> LookupLineLength) lookups)
    {
        var first = (X: points[firstIndex][0], Y: points[firstIndex][1]);
        var second = (X: points[secondIndex][0], Y: points[secondIndex][1]);
        var dx = second.X - first.X;
        var dy = second.Y - first.Y;
        var gcd = Gcd(dx, dy);
        var (a, b) = (dx / gcd, dy / gcd);

        if (IsInOppositeHalfPlane(a, b))
        {
            (a, b) = (-a, -b);
        }

        var c = b * first.X - a * first.Y;
        var length = dx * dx + dy * dy;

        var counted = CountThenIncrement(lookups.LookupSlope, (a, b)) - CountThenIncrement(lookups.LookupLine, (a, b, c));
        var duplicate = CountThenIncrement(lookups.LookupSlopeLength, (a, b, length)) -
            CountThenIncrement(lookups.LookupLineLength, (a, b, c, length));

        return (counted, duplicate);
    }

    private static int Gcd(int left, int right)
    {
        while (right != 0)
        {
            (left, right) = (right, left % right);
        }

        return Math.Abs(left);
    }

    // Canonical directions point the positive-x way: directionX > 0, or
    // directionX == 0 with directionY > 0. A direction in the opposite half is
    // negated to match, so the two orientations of one slope collapse onto a single
    // key.
    private static bool IsInOppositeHalfPlane(int directionX, int directionY)
        => directionX < 0 || (directionX == 0 && directionY < 0);

    private static int CountThenIncrement<TKey>(HashMap<TKey, int> lookup, TKey key)
    {
        lookup.TryGetValue(key, out var count);
        lookup.Set(key, count + 1);

        return count;
    }
}
