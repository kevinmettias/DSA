using DSAExperimentation.LeetCode.MaximumPartitionFactor;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumPartitionFactor;

// Harness only. Both strategies are MaximumPartitionFactorSolution's - this file
// pins them to LeetCode's published examples.
public sealed class MaximumPartitionFactorTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[0, 0], [0, 2], [2, 0], [2, 2]], 4 },
            { [[0, 0], [0, 1], [10, 0]], 11 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPartitionFactorByLinearScan_LeetCodeExamples_ReturnsMaximumPartitionFactor(
        int[][] points, int expected) =>
        Assert.Equal(expected, MaximumPartitionFactorSolution.MaxPartitionFactorByLinearScan(points));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPartitionFactorByBinarySearchBipartiteCheck_LeetCodeExamples_ReturnsMaximumPartitionFactor(
        int[][] points, int expected) =>
        Assert.Equal(expected, MaximumPartitionFactorSolution.MaxPartitionFactorByBinarySearchBipartiteCheck(points));
}
