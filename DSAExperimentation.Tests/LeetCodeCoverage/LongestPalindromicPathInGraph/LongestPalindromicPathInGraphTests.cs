using DSAExperimentation.LeetCode.LongestPalindromicPathInGraph;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestPalindromicPathInGraph;

// Harness only. Both strategies are LongestPalindromicPathInGraphSolution's -
// this file just pins them to LeetCode's published examples.
public sealed class LongestPalindromicPathInGraphTests
{
    public static TheoryData<int, int[][], string, int> Examples =>
        new()
        {
            { 3, [[0, 1], [1, 2]], "aba", 3 },
            { 3, [[0, 1], [0, 2]], "abc", 1 },
            { 4, [[0, 2], [0, 3], [3, 1]], "bbac", 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestPalindromeByBruteForceDfs_LeetCodeExamples_ReturnsLongestPalindromicPathLength(
        int n, int[][] edges, string label, int expected) =>
        Assert.Equal(
            expected, LongestPalindromicPathInGraphSolution.LongestPalindromeByBruteForceDfs(n, edges, label));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestPalindromeByBitmaskMemo_LeetCodeExamples_ReturnsLongestPalindromicPathLength(
        int n, int[][] edges, string label, int expected) =>
        Assert.Equal(
            expected, LongestPalindromicPathInGraphSolution.LongestPalindromeByBitmaskMemo(n, edges, label));
}
