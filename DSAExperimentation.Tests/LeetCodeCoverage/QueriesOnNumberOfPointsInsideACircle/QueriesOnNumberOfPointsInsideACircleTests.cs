using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.QueriesOnNumberOfPointsInsideACircle;

// LeetCode 1828. Queries on Number of Points Inside a Circle: sort the points by x
// once, then for each query use BinarySearch.LowerBound/UpperBound (over an
// ArraySequence<int> of the sorted x-coordinates, this repo's own O(1)-Get
// IRandomAccessSequence<Element> witness) to narrow to only the points whose x falls
// within [qx-r, qx+r] before checking the (still necessary) squared-distance test -
// pruning the O(points) per-query scan down to just its x-range instead of scanning
// every point regardless of how far away it is.
public sealed partial class QueriesOnNumberOfPointsInsideACircleTests
{
    [Fact]
    public void CountPoints_ClassicDiagonalLine_ReturnsExpectedCounts()
    {
        int[][] points = [[1, 1], [2, 2], [3, 3], [4, 4], [5, 5]];
        int[][] queries = [[3, 3, 2], [0, 0, 2], [10, 10, 1]];

        var result = CountPoints(points, queries);

        Assert.Equal([3, 1, 0], result);
    }

    [Fact]
    public void CountPoints_QueryExactlyOnPoint_ZeroRadiusStillCountsIt()
    {
        int[][] points = [[0, 0]];
        int[][] queries = [[0, 0, 0]];

        var result = CountPoints(points, queries);

        Assert.Equal([1], result);
    }

    [Fact]
    public void CountPoints_NegativeCoordinates_ComputesSquaredDistanceCorrectly()
    {
        int[][] points = [[-3, -3], [-1, -1], [1, 1]];
        int[][] queries = [[-2, -2, 3]];

        var result = CountPoints(points, queries);

        Assert.Equal([2], result);
    }

    private static int[] CountPoints(int[][] points, int[][] queries)
    {
        var sortedPoints = points.OrderBy(point => point[0]).ToArray();
        var xs = sortedPoints.Select(point => point[0]).ToArray();
        var sequence = new ArraySequence<int>(xs);
        var results = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            results[q] = CountPointsForQuery(queries[q], sortedPoints, sequence);
        }

        return results;
    }

    private static int CountPointsForQuery(int[] query, int[][] sortedPoints, ArraySequence<int> sequence)
    {
        var x = query[0];
        var y = query[1];
        var r = query[2];
        var lo = BinarySearch.LowerBound(sequence, x - r);
        var hi = BinarySearch.UpperBound(sequence, x + r);
        var count = 0;

        for (var i = lo; i < hi; i++)
        {
            var dx = sortedPoints[i][0] - x;
            var dy = sortedPoints[i][1] - y;

            if ((dx * dx) + (dy * dy) <= r * r)
            {
                count++;
            }
        }

        return count;
    }
}
