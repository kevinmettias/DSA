using DSAExperimentation.LeetCode.KthSmallestPathXORSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthSmallestPathXORSum;

// Harness only: the tree is DataStructures' own ParentArrayTree/RootedTreeNode
// and both strategies are KthSmallestPathXORSumSolution's - this file just pins
// them to LeetCode's published examples (CheckIfDfsStringsArePalindromesTests
// precedent).
public sealed class KthSmallestPathXORSumTests
{
    public static TheoryData<int[], int[], int[][], int[]> Examples =>
        new()
        {
            {
                [-1, 0, 0], [1, 1, 1], [[0, 1], [0, 2], [0, 3]],
                [0, 1, -1]
            },
            {
                [-1, 0, 1], [5, 2, 7], [[0, 1], [1, 2], [1, 3], [2, 1]],
                [0, 7, -1, 0]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthSmallestXorSumByPerQueryWalk_LeetCodeExamples_ReturnsKthDistinctSubtreeXorSum(
        int[] par, int[] vals, int[][] queries, int[] expected) =>
        Assert.Equal(expected, KthSmallestPathXORSumSolution.KthSmallestXorSumByPerQueryWalk(par, vals, queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthSmallestXorSumByEulerTourCache_LeetCodeExamples_ReturnsKthDistinctSubtreeXorSum(
        int[] par, int[] vals, int[][] queries, int[] expected) =>
        Assert.Equal(expected, KthSmallestPathXORSumSolution.KthSmallestXorSumByEulerTourCache(par, vals, queries));
}
