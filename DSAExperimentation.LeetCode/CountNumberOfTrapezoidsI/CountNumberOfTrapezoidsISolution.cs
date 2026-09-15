using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountNumberOfTrapezoidsI;

// LeetCode 3623. Count Number of Trapezoids I: a horizontal trapezoid is 4
// points that split into two pairs each sharing a y-coordinate, the two
// y-coordinates themselves distinct. Both strategies answer the same question
// with the same signature (TwoSumSolution precedent).
internal static class CountNumberOfTrapezoidsISolution
{
    // The textbook C(n, 4) scan of every 4-point combination, checking all
    // three ways to split them into two same-y pairs. Correct, and the arm the
    // grouped-by-y strategy below has to beat - only usable at small n, since
    // C(n, 4) already reaches into the millions past a few hundred points.
    public static int CountTrapezoidsByBruteForce(int[][] points)
    {
        var n = points.Length;
        var count = 0L;

        for (var a = 0; a < n; a++)
        {
            for (var b = a + 1; b < n; b++)
            {
                count += CountTrapezoidsWithInitialPair(points, a, b);
            }
        }

        return (int)(count % ModularArithmetic.Modulo);
    }

    // The completions of one fixed first pair: every 4-subset whose two lowest
    // indices are firstIndex and secondIndex is finished by a pair (c, d) drawn
    // from the points after secondIndex, and contributes exactly when those four
    // y-coordinates split into two same-y pairs. It is a method of its own rather
    // than the tail of the scan above because the four-index enumeration and the
    // test want four levels of control flow between them, one past the
    // nesting-depth cap.
    private static long CountTrapezoidsWithInitialPair(int[][] points, int firstIndex, int secondIndex)
    {
        var count = 0L;

        for (var c = secondIndex + 1; c < points.Length; c++)
        {
            for (var d = c + 1; d < points.Length; d++)
            {
                if (IsHorizontalTrapezoid(
                    points[firstIndex][1], points[secondIndex][1], points[c][1], points[d][1]))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool IsHorizontalTrapezoid(int y1, int y2, int y3, int y4) =>
        (y1 == y2 && y3 == y4 && y1 != y3) ||
        (y1 == y3 && y2 == y4 && y1 != y2) ||
        (y1 == y4 && y2 == y3 && y1 != y2);

    // Every horizontal side is a pair of points sharing a y; grouping points by
    // y (this repo's own HashMap<TKey, TValue>, the TwoSumSolution precedent)
    // turns "count 4-point splits into two distinct-y pairs" into "count
    // unordered pairs of horizontal-segment counts across distinct y groups" -
    // (sum^2 - sum-of-squares) / 2, computed via ModularArithmetic's
    // multiplicative inverse of 2 (Domain.Modular's own nCr-by-modular-inverse
    // shape, applied here to a division instead of a binomial coefficient)
    // rather than exact integer division, since the running sums are already
    // reduced mod 1e9+7.
    public static int CountTrapezoidsByHorizontalPairCounting(int[][] points)
    {
        var countsByY = BuildCountsByY(points);
        var pairCount = CountPairsAcrossGroups(countsByY);

        return (int)pairCount;
    }

    // How many points share each y-coordinate: a group of count points on one line
    // offers C(count, 2) horizontal segments to pair with another group's.
    private static HashMap<int, long> BuildCountsByY(int[][] points)
    {
        var countsByY = new HashMap<int, long>();

        foreach (var point in points)
        {
            countsByY.TryGetValue(point[1], out var count);
            countsByY.Set(point[1], count + 1);
        }

        return countsByY;
    }

    // Every unordered pair of segments drawn from two DIFFERENT y groups, which is the
    // (sum^2 - sum-of-squares) / 2 above: the two running totals are reduced mod 1e9+7
    // as they accumulate, so the halving goes through the multiplicative inverse too.
    private static long CountPairsAcrossGroups(HashMap<int, long> countsByY)
    {
        var sum = 0L;
        var sumOfSquares = 0L;

        foreach (var count in countsByY.Values)
        {
            var segments = count * (count - 1) / 2 % ModularArithmetic.Modulo;
            sum = (sum + segments) % ModularArithmetic.Modulo;
            sumOfSquares = (sumOfSquares + segments * segments % ModularArithmetic.Modulo) % ModularArithmetic.Modulo;
        }

        var totalSquared = sum * sum % ModularArithmetic.Modulo;
        var diff = (totalSquared - sumOfSquares + ModularArithmetic.Modulo) % ModularArithmetic.Modulo;
        var inverseOfTwo = ModularArithmetic.Inverse(2);

        return diff * inverseOfTwo % ModularArithmetic.Modulo;
    }
}
