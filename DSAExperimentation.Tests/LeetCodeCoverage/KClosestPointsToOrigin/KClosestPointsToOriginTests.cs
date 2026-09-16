using DSAExperimentation.LeetCode.KClosestPointsToOrigin;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KClosestPointsToOrigin;

// Harness only: both strategies live in KClosestPointsToOriginSolution. LeetCode lets
// the answer come back in any order - the full sort returns nearest-first, the size-k
// max-heap returns farthest-first as it drains - so every example is asserted as a set
// of points, and no example is written with a distance tie straddling the nearestCount
// boundary, which would make more than one answer correct.
public sealed class KClosestPointsToOriginTests
{
    public static TheoryData<int[][], int, int[][]> Examples =>
        new()
        {
            { [[1, 3], [-2, 2]], 1, [[-2, 2]] },
            { [[3, 3], [5, -1], [-2, 4]], 2, [[3, 3], [-2, 4]] },
            { [[3, 3], [5, -1], [-2, 4]], 3, [[3, 3], [5, -1], [-2, 4]] },
            { [[-5, 4], [-6, -5], [4, 6]], 2, [[-5, 4], [4, 6]] },
            { [[2, 2]], 1, [[2, 2]] },
            { [[0, 1], [1, 0]], 2, [[0, 1], [1, 0]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void KClosestByFullSort_LeetCodeExamples_ReturnsTheKNearestPoints(
        int[][] points, int nearestCount, int[][] expected)
    {
        var nearest = KClosestPointsToOriginSolution.KClosestByFullSort(points, nearestCount);

        Assert.Equal(ToSet(expected), ToSet(nearest));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void KClosestBySizeKMaxHeap_LeetCodeExamples_ReturnsTheKNearestPoints(
        int[][] points, int nearestCount, int[][] expected)
    {
        var nearest = KClosestPointsToOriginSolution.KClosestBySizeKMaxHeap(points, nearestCount);

        Assert.Equal(ToSet(expected), ToSet(nearest));
    }

    private static HashSet<(int X, int Y)> ToSet(int[][] points) => [.. points.Select(p => (p[0], p[1]))];
}
