using DSAExperimentation.LeetCode.MinimizeManhattanDistances;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimizeManhattanDistances;

// Harness only. The u/v transform and both search strategies are
// MinimizeManhattanDistancesSolution's - this file just pins them to LeetCode's
// published examples, including the all-coincident-points case that forces every
// transform range to collapse to zero.
public sealed class MinimizeManhattanDistancesTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            {
                [[3, 10], [5, 15], [10, 2], [4, 4]],
                12
            },
            {
                [[1, 1], [1, 1], [1, 1]],
                0
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDistanceByBruteForce_LeetCodeExamples_ReturnsMinimizedMaximumDistance(
        int[][] points, int expected) =>
        Assert.Equal(expected, MinimizeManhattanDistancesSolution.MinDistanceByBruteForce(points));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDistanceByManhattanTransform_LeetCodeExamples_ReturnsMinimizedMaximumDistance(
        int[][] points, int expected) =>
        Assert.Equal(expected, MinimizeManhattanDistancesSolution.MinDistanceByManhattanTransform(points));
}
