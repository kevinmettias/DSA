using DSAExperimentation.LeetCode.CountConnectedSubgraphsWithEvenNodeSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountConnectedSubgraphsWithEvenNodeSum;

// Harness only. Both strategies are
// CountConnectedSubgraphsWithEvenNodeSumSolution's - this file just pins them to
// LeetCode's published examples, including the single-node graph where the only
// possible subset has an odd sum.
public sealed class CountConnectedSubgraphsWithEvenNodeSumTests
{
    public static TheoryData<int[], int[][], int> Examples =>
        new()
        {
            { [1, 0, 1], [[0, 1], [1, 2]], 2 },
            { [1], [], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountEvenSumSubgraphsByBruteForceBfs_LeetCodeExamples_ReturnsConnectedEvenSumSubsetCount(
        int[] nums, int[][] edges, int expected)
    {
        var actual = CountConnectedSubgraphsWithEvenNodeSumSolution.CountEvenSumSubgraphsByBruteForceBfs(nums, edges);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountEvenSumSubgraphsByDisjointSet_LeetCodeExamples_ReturnsConnectedEvenSumSubsetCount(
        int[] nums, int[][] edges, int expected)
    {
        var actual = CountConnectedSubgraphsWithEvenNodeSumSolution.CountEvenSumSubgraphsByDisjointSet(nums, edges);
        Assert.Equal(expected, actual);
    }
}
