using DSAExperimentation.LeetCode.QueryKthSmallestTrimmedNumber;

namespace DSAExperimentation.Tests.LeetCodeCoverage.QueryKthSmallestTrimmedNumber;

// Harness only: both strategies live in QueryKthSmallestTrimmedNumberSolution and answer the
// same query batches here under their own names, so a failure names the strategy that broke.
// The tie case pins the rule the whole problem turns on - equal trimmed values resolve toward
// the smaller original index - which the selection scan gets from scanning ascending and the
// sort gets from MergeSort's documented stability.
public sealed class QueryKthSmallestTrimmedNumberTests
{
    public static TheoryData<string[], int[][], int[]> Examples => new()
    {
        { ["102", "473", "251", "814"], [[1, 1], [2, 3], [4, 2], [1, 2]], [2, 2, 1, 0] },
        { ["24", "37", "96", "04"], [[2, 1], [2, 2]], [3, 0] },
        { ["11", "11"], [[1, 1], [2, 1]], [0, 1] },
        { ["456", "123", "789"], [[1, 3], [3, 3]], [1, 2] },
        { ["7"], [[1, 1]], [0] },
    };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AnswerQueriesBySelectionScan_Example_ReturnsOriginalIndexOfKthSmallestTrimmed(
        string[] nums, int[][] queries, int[] expected)
    {
        var actual = QueryKthSmallestTrimmedNumberSolution.AnswerQueriesBySelectionScan(nums, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void AnswerQueriesByMergeSort_Example_ReturnsOriginalIndexOfKthSmallestTrimmed(
        string[] nums, int[][] queries, int[] expected)
    {
        var actual = QueryKthSmallestTrimmedNumberSolution.AnswerQueriesByMergeSort(nums, queries);

        Assert.Equal(expected, actual);
    }
}
