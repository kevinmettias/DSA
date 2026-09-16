using DSAExperimentation.LeetCode.OnlineMajorityElementInSubarray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OnlineMajorityElementInSubarray;

// Harness only. Both strategies are OnlineMajorityElementInSubarraySolution's - this
// file builds each example's checker once and replays that example's whole query
// sequence against it, which is what LeetCode's Design shape actually specifies, so a
// failure still names the strategy that broke. Every query obeys LeetCode's own
// 2*threshold > right-left+1 guarantee, so each has exactly one admissible answer.
public sealed class OnlineMajorityElementInSubarrayTests
{
    public static TheoryData<int[], int[][], int[]> Examples =>
        new()
        {
            // LeetCode's published example.
            {
                [1, 1, 2, 2, 1, 1],
                [[0, 5, 4], [0, 3, 3], [2, 3, 2]],
                [1, -1, 2]
            },
            // A single-index range is always its own majority.
            {
                [5, 5, 5, 5, 5],
                [[2, 2, 1], [0, 4, 3], [1, 3, 2]],
                [5, 5, 5]
            },
            // All distinct: nothing can clear a threshold above one.
            {
                [1, 2, 3, 4],
                [[0, 2, 2], [1, 3, 2], [3, 3, 1]],
                [-1, -1, 4]
            },
            // The majority element differs from range to range.
            {
                [1, 1, 1, 2, 2],
                [[0, 4, 3], [3, 4, 2], [1, 3, 2]],
                [1, 2, 1]
            },
            // Single-element array, the smallest checker LeetCode admits.
            {
                [7],
                [[0, 0, 1]],
                [7]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MajorityCheckerByRangeTally_LeetCodeExamples_ReturnsMajorityOrNegativeOne(
        int[] arr, int[][] queries, int[] expected) =>
        AssertQueryResults(
            new OnlineMajorityElementInSubarraySolution.MajorityCheckerByRangeTally(arr),
            queries,
            expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void MajorityCheckerByPositionIndex_LeetCodeExamples_ReturnsMajorityOrNegativeOne(
        int[] arr, int[][] queries, int[] expected) =>
        AssertQueryResults(
            new OnlineMajorityElementInSubarraySolution.MajorityCheckerByPositionIndex(arr),
            queries,
            expected);

    private static void AssertQueryResults(
        OnlineMajorityElementInSubarraySolution.IMajorityChecker checker,
        int[][] queries,
        int[] expected)
    {
        for (var i = 0; i < queries.Length; i++)
        {
            var query = queries[i];
            var majority = checker.Query(query[0], query[1], query[2]);

            Assert.Equal(expected[i], majority);
        }
    }
}
