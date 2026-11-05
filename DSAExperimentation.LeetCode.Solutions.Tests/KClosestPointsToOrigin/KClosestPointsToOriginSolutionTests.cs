using DSAExperimentation.LeetCode.Solutions.KClosestPointsToOrigin;

namespace DSAExperimentation.LeetCode.Solutions.Tests.KClosestPointsToOrigin;

// Harness only: both strategies live in KClosestPointsToOriginSolution. LeetCode lets
// the answer come back in any order - the full sort returns nearest-first, the size-k
// max-heap returns farthest-first as it drains - so every case is asserted as a set
// of points, and no case is written with a distance tie straddling the nearestCount
// boundary, which would make more than one answer correct.
public sealed partial class KClosestPointsToOriginSolutionTests
{
    public static TheoryData<int[][], int, int[][]> Cases =>
        new()
        {
            { [[3, 3], [5, -1], [-2, 4]], 3, [[3, 3], [5, -1], [-2, 4]] },
            { [[-5, 4], [-6, -5], [4, 6]], 2, [[-5, 4], [4, 6]] },
            { [[2, 2]], 1, [[2, 2]] },

            // k equal to the point count: every point is returned, nothing is cut.
            { [[1, 1], [2, 2]], 2, [[1, 1], [2, 2]] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void KClosestByFullSort_Cases_ReturnsTheKNearestPoints(
        int[][] points, int nearestCount, int[][] expected)
    {
        var nearest = KClosestPointsToOriginSolution.KClosestByFullSort(points, nearestCount);

        Assert.Equal(ToSet(expected), ToSet(nearest));
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void KClosestBySizeKMaxHeap_Cases_ReturnsTheKNearestPoints(
        int[][] points, int nearestCount, int[][] expected)
    {
        var nearest = KClosestPointsToOriginSolution.KClosestBySizeKMaxHeap(points, nearestCount);

        Assert.Equal(ToSet(expected), ToSet(nearest));
    }

    private static HashSet<(int X, int Y)> ToSet(int[][] points) => [.. points.Select(p => (p[0], p[1]))];
}
