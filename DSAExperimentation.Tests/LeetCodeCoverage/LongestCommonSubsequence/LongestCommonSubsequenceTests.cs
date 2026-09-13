using DSAExperimentation.LeetCode.LongestCommonSubsequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestCommonSubsequence;

// Harness only. Both strategies are LongestCommonSubsequenceSolution's -
// LengthByTabulation (previously untested scaffolding inlined in the benchmark as its
// baseline arm) now gets the same examples as LengthByMemoizedSuffixPairDp
// (previously the test's own private helper), so a failure names the strategy that
// broke.
public sealed class LongestCommonSubsequenceTests
{
    public static TheoryData<string, string, int> Examples =>
        new()
        {
            { "abcde", "ace", 3 },
            { "abc", "abc", 3 },
            { "abc", "def", 0 },
            { "abcba", "abcbcba", 5 },
            { "ezupkr", "ubmrapg", 2 },
            { "aaaa", "aa", 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LengthByTabulation_LeetCodeExamples_ReturnsLcsLength(
        string text1, string text2, int expected) =>
        Assert.Equal(expected, LongestCommonSubsequenceSolution.LengthByTabulation(text1, text2));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LengthByMemoizedSuffixPairDp_LeetCodeExamples_ReturnsLcsLength(
        string text1, string text2, int expected) =>
        Assert.Equal(expected, LongestCommonSubsequenceSolution.LengthByMemoizedSuffixPairDp(text1, text2));
}
