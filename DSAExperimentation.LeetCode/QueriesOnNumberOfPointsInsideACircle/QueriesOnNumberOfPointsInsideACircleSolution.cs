using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.QueriesOnNumberOfPointsInsideACircle;

// LeetCode 1828. Queries on Number of Points Inside a Circle: for each query
// [x, y, r], how many of the given points lie inside or on that circle.
//
// Both strategies decide membership with the identical squared-distance test below,
// so they differ only in how many points they bother to test: the baseline tests
// every point for every query, the composed strategy first narrows to the points
// whose x-coordinate can possibly be within r.
internal static class QueriesOnNumberOfPointsInsideACircleSolution
{
    private const int XIndex = 0;
    private const int YIndex = 1;
    private const int RadiusIndex = 2;

    // The textbook answer: for every query, scan every point. Deliberately written
    // without this repo's primitives - it is the arm the composed strategy below has
    // to justify itself against.
    public static int[] CountPointsByBruteForceScan(int[][] points, int[][] queries)
    {
        var results = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            results[q] = CountEveryPoint(queries[q], points);
        }

        return results;
    }

    private static int CountEveryPoint(int[] query, int[][] points)
    {
        var count = 0;

        foreach (var point in points)
        {
            if (Covers(query, point))
            {
                count++;
            }
        }

        return count;
    }

    // Sort the points by x once, then let BinarySearch.LowerBound/UpperBound over an
    // ArraySequence<int> of the sorted x-coordinates (this repo's own O(1)-Get
    // IRandomAccessSequence<Element> witness) clip each query down to the half-open
    // index range whose x falls in [qx - r, qx + r]. Everything outside that band is
    // already further than r away in x alone, so the (still necessary)
    // squared-distance test only ever runs on candidates.
    public static int[] CountPointsBySortedXPruning(int[][] points, int[][] queries)
    {
        var sortedPoints = points.OrderBy(point => point[XIndex]).ToArray();
        var xs = sortedPoints.Select(point => point[XIndex]).ToArray();
        var sequence = new ArraySequence<int>(xs);
        var results = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            results[q] = CountWithinXBand(queries[q], sortedPoints, sequence);
        }

        return results;
    }

    private static int CountWithinXBand(int[] query, int[][] sortedPoints, ArraySequence<int> sequence)
    {
        var low = BinarySearch.LowerBound(sequence, query[XIndex] - query[RadiusIndex]);
        var high = BinarySearch.UpperBound(sequence, query[XIndex] + query[RadiusIndex]);
        var count = 0;

        for (var i = low; i < high; i++)
        {
            if (Covers(query, sortedPoints[i]))
            {
                count++;
            }
        }

        return count;
    }

    // Squared distance, never a square root: comparing dx^2 + dy^2 against r^2 keeps
    // the whole test in exact integer arithmetic. It is widened to long because a
    // coordinate spread wider than LeetCode's own [-1000, 1000] constraint - which is
    // exactly what the benchmark workload uses to make the x-band prune matter -
    // overflows a 32-bit square.
    private static bool Covers(int[] query, int[] point)
    {
        long dx = point[XIndex] - query[XIndex];
        long dy = point[YIndex] - query[YIndex];
        long radius = query[RadiusIndex];

        return (dx * dx) + (dy * dy) <= radius * radius;
    }
}
