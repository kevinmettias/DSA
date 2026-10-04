using DSAExperimentation.LeetCode.MinimizeManhattanDistances;

namespace DSAExperimentation.LeetCode.Tests.MinimizeManhattanDistances;

// Harness only. The u/v transform and both search strategies are
// MinimizeManhattanDistancesSolution's - this file just pins them to LeetCode's
// published examples, including the all-coincident-points case that forces every
// transform range to collapse to zero. The two sorted transforms the second strategy
// is handed are asserted on their own.
public sealed partial class MinimizeManhattanDistancesSolutionTests
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

    // LeetCode's first example, point by point: u = x + y is 13, 20, 12, 8 and
    // v = x - y is -7, -10, 8, 0 for points 0..3. Sorted, u runs 8 (point 3), 12 (2),
    // 13 (0), 20 (1) and v runs -10 (1), -7 (0), 0 (3), 8 (2).
    [Fact]
    public void BuildSortedTransforms_LeetCodeFirstExample_SortsBothRotatedAxesKeepingEachPointIndex()
    {
        var (sortedByU, sortedByV) = MinimizeManhattanDistancesSolution.BuildSortedTransforms(
            [[3, 10], [5, 15], [10, 2], [4, 4]]);

        Assert.Equal([(8L, 3), (12L, 2), (13L, 0), (20L, 1)], sortedByU);
        Assert.Equal([(-10L, 1), (-7L, 0), (0L, 3), (8L, 2)], sortedByV);
    }
}
