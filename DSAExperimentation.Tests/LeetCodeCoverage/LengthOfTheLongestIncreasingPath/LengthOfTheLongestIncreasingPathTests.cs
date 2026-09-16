using DSAExperimentation.LeetCode.LengthOfTheLongestIncreasingPath;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LengthOfTheLongestIncreasingPath;

// Harness only. Both search strategies are LengthOfTheLongestIncreasingPathSolution's -
// this file just pins them to LeetCode's published examples.
public sealed class LengthOfTheLongestIncreasingPathTests
{
    public static TheoryData<int[][], int, int> Examples =>
        new()
        {
            { [[3, 1], [2, 2], [4, 1], [0, 0], [5, 3]], 1, 3 },
            { [[2, 1], [7, 0], [5, 6]], 2, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPathLengthByBruteForce_LeetCodeExamples_ReturnsLongestIncreasingPathThroughK(
        int[][] coordinates, int requiredIndex, int expected)
    {
        var actual = LengthOfTheLongestIncreasingPathSolution.MaxPathLengthByBruteForce(
            coordinates, requiredIndex);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPathLengthBySegmentTree_LeetCodeExamples_ReturnsLongestIncreasingPathThroughK(
        int[][] coordinates, int requiredIndex, int expected)
    {
        var actual = LengthOfTheLongestIncreasingPathSolution.MaxPathLengthBySegmentTree(
            coordinates, requiredIndex);

        Assert.Equal(expected, actual);
    }
}
