using DSAExperimentation.LeetCode.MinimumOperationsToEqualizeSubarrays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumOperationsToEqualizeSubarrays;

// Harness only. Both strategies are MinimumOperationsToEqualizeSubarraysSolution's -
// this file just pins them to LeetCode's published examples, including the
// [0,2] query that must resolve to -1 because index 0 and index 2 sit in
// different remainder-mod-k runs.
public sealed class MinimumOperationsToEqualizeSubarraysTests
{
    public static TheoryData<int[], int, int[][], long[]> Examples =>
        new()
        {
            { [1, 4, 7], 3, [[0, 1], [0, 2]], [1, 2] },
            { [1, 2, 4], 2, [[0, 2], [0, 0], [1, 2]], [-1, 0, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByBruteForce_LeetCodeExamples_ReturnsPerQueryOperationCounts(
        int[] nums, int k, int[][] queries, long[] expected)
    {
        var actual = MinimumOperationsToEqualizeSubarraysSolution.MinOperationsByBruteForce(nums, queries, k);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByMergeSortTree_LeetCodeExamples_ReturnsPerQueryOperationCounts(
        int[] nums, int k, int[][] queries, long[] expected)
    {
        var actual = MinimumOperationsToEqualizeSubarraysSolution.MinOperationsByMergeSortTree(nums, queries, k);
        Assert.Equal(expected, actual);
    }
}
