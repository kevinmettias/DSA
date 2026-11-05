using DSAExperimentation.LeetCode.Solutions.QueriesOnNumberOfPointsInsideACircle;

namespace DSAExperimentation.LeetCode.Solutions.Tests.QueriesOnNumberOfPointsInsideACircle;

// Harness only. Both strategies are QueriesOnNumberOfPointsInsideACircleSolution's -
// this file pins them to the boundary cases
// the pre-migration test already carried: a zero radius that still covers the point
// it sits on, and negative coordinates whose squared distance must not lose its sign
// handling. The brute-force scan was previously untested scaffolding inlined in the
// benchmark and is asserted here for the first time.
public sealed partial class QueriesOnNumberOfPointsInsideACircleSolutionTests
{
    public static TheoryData<int[][], int[][], int[]> Cases =>
        new()
        {
            // A query centred on a point, one offset from every point, and one that
            // reaches nothing at all - so an empty x-band is exercised too.
            { [[1, 1], [2, 2], [3, 3], [4, 4], [5, 5]], [[3, 3, 2], [0, 0, 2], [10, 10, 1]], [3, 1, 0] },

            // Radius zero still covers the point the circle is centred on.
            { [[0, 0]], [[0, 0, 0]], [1] },

            // Negative coordinates: the squared distance has to be computed from the
            // signed differences, not their magnitudes in one axis only.
            { [[-3, -3], [-1, -1], [1, 1]], [[-2, -2, 3]], [2] },

            // Points sharing an x-coordinate, so the sorted-x band spans a run of
            // equal keys and UpperBound has to include all of it.
            { [[2, 0], [2, 5], [2, -5], [9, 0]], [[2, 0, 5], [2, 0, 4]], [3, 1] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountPointsByBruteForceScan_Cases_CountsPointsInsideEachCircle(
        int[][] points, int[][] queries, int[] expected)
    {
        var actual = QueriesOnNumberOfPointsInsideACircleSolution.CountPointsByBruteForceScan(points, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountPointsBySortedXPruning_Cases_CountsPointsInsideEachCircle(
        int[][] points, int[][] queries, int[] expected)
    {
        var actual = QueriesOnNumberOfPointsInsideACircleSolution.CountPointsBySortedXPruning(points, queries);

        Assert.Equal(expected, actual);
    }
}
