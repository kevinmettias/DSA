using DSAExperimentation.LeetCode.MinimumOperationsToEqualizeSubarrays;

namespace DSAExperimentation.LeetCode.Tests.MinimumOperationsToEqualizeSubarrays;

// Harness only. Both strategies are MinimumOperationsToEqualizeSubarraysSolution's -
// this file just pins them to LeetCode's published examples, including the
// [0,2] query that must resolve to -1 because index 0 and index 2 sit in
// different remainder-mod-k runs. The run ids and the merge-sort tree the second
// strategy is handed are asserted on their own.
public sealed partial class MinimumOperationsToEqualizeSubarraysSolutionTests
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
        int[] nums, int stepSize, int[][] queries, long[] expected)
    {
        var actual = MinimumOperationsToEqualizeSubarraysSolution.MinOperationsByBruteForce(nums, queries, stepSize);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByMergeSortTree_LeetCodeExamples_ReturnsPerQueryOperationCounts(
        int[] nums, int stepSize, int[][] queries, long[] expected)
    {
        var actual = MinimumOperationsToEqualizeSubarraysSolution.MinOperationsByMergeSortTree(nums, queries, stepSize);
        Assert.Equal(expected, actual);
    }

    // LeetCode's second example: 1, 2, 4 leave remainders 1, 0, 0 mod 2, so a new run
    // starts at index 1 and index 2 stays in it.
    [Fact]
    public void BuildIndex_LeetCodeSecondExample_StartsANewRunWhereTheRemainderChanges()
    {
        var index = MinimumOperationsToEqualizeSubarraysSolution.BuildIndex([1, 2, 4], 2);

        Assert.Equal([0, 1, 1], index.RunId);
    }

    // LeetCode's first example reversed, so the tree has something to sort: 7, 4, 1 all
    // leave remainder 1 mod 3, one run throughout, and each queried window comes back in
    // ascending order - [7, 4, 1] as 1, 4, 7, [7, 4] as 4, 7 and [4, 1] as 1, 4.
    [Fact]
    public void BuildIndex_LeetCodeFirstExampleReversed_TreeReturnsEachWindowSorted()
    {
        var index = MinimumOperationsToEqualizeSubarraysSolution.BuildIndex([7, 4, 1], 3);
        var whole = index.Tree.Query(0, 2);
        var leftPair = index.Tree.Query(0, 1);
        var rightPair = index.Tree.Query(1, 2);

        Assert.Equal([0, 0, 0], index.RunId);
        Assert.Equal([1, 4, 7], whole);
        Assert.Equal([4, 7], leftPair);
        Assert.Equal([1, 4], rightPair);
    }
}
