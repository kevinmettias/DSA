using DSAExperimentation.LeetCode.QueriesOnNumberOfPointsInsideACircle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.QueriesOnNumberOfPointsInsideACircle;

// Harness only. Both strategies are QueriesOnNumberOfPointsInsideACircleSolution's -
// this file pins them to LeetCode's two published examples plus the boundary cases
// the pre-migration test already carried: a zero radius that still covers the point
// it sits on, and negative coordinates whose squared distance must not lose its sign
// handling. The brute-force scan was previously untested scaffolding inlined in the
// benchmark and is asserted here for the first time.
public sealed class QueriesOnNumberOfPointsInsideACircleTests
{
    public static TheoryData<int[][], int[][], int[]> Examples =>
        new()
        {
            // LeetCode example 1.
            { [[1, 3], [3, 3], [5, 3], [2, 2]], [[2, 3, 1], [4, 3, 1], [1, 1, 2]], [3, 2, 2] },

            // LeetCode example 2.
            {
                [[1, 1], [2, 2], [3, 3], [4, 4], [5, 5]],
                [[1, 2, 2], [2, 2, 2], [4, 3, 2], [4, 3, 3]],
                [2, 3, 2, 4]
            },

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
    [MemberData(nameof(Examples))]
    public void CountPointsByBruteForceScan_LeetCodeExamples_CountsPointsInsideEachCircle(
        int[][] points, int[][] queries, int[] expected) =>
        Assert.Equal(
            expected,
            QueriesOnNumberOfPointsInsideACircleSolution.CountPointsByBruteForceScan(points, queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPointsBySortedXPruning_LeetCodeExamples_CountsPointsInsideEachCircle(
        int[][] points, int[][] queries, int[] expected) =>
        Assert.Equal(
            expected,
            QueriesOnNumberOfPointsInsideACircleSolution.CountPointsBySortedXPruning(points, queries));
}
