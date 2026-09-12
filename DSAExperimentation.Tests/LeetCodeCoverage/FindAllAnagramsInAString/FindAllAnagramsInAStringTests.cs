using DSAExperimentation.LeetCode.FindAllAnagramsInAString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindAllAnagramsInAString;

// Harness only. Both strategies are FindAllAnagramsInAStringSolution's - this file
// just pins them to LeetCode's published examples, including the pattern-longer-
// than-string case both strategies must short-circuit on.
public sealed class FindAllAnagramsInAStringTests
{
    public static TheoryData<string, string, int[]> Examples =>
        new()
        {
            { "cbaebabacd", "abc", [0, 6] },
            { "abab", "ab", [0, 1, 2] },
            { "a", "aa", [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindAnagramIndicesByBruteForceRebuild_LeetCodeExamples_ReturnsEveryAnagramStart(
        string s, string p, int[] expected) =>
        Assert.Equal(
            expected, FindAllAnagramsInAStringSolution.FindAnagramIndicesByBruteForceRebuild(s, p));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindAnagramIndicesBySlidingWindow_LeetCodeExamples_ReturnsEveryAnagramStart(
        string s, string p, int[] expected) =>
        Assert.Equal(
            expected, FindAllAnagramsInAStringSolution.FindAnagramIndicesBySlidingWindow(s, p));
}
