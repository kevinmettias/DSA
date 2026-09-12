using DSAExperimentation.LeetCode.PermutationInString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PermutationInString;

// Harness only. Both frequency-window strategies are
// PermutationInStringSolution's - this file just pins them to LeetCode's
// published examples plus a couple of edge cases (s1 longer than s2, and a
// match that only appears once the window has slid past the start).
public sealed class PermutationInStringTests
{
    public static TheoryData<string, string, bool> Examples =>
        new()
        {
            { "ab", "eidbaooo", true },
            { "ab", "eidboaoo", false },
            { "abc", "ab", false },
            { "adc", "dcda", true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckInclusionByPerWindowRebuild_LeetCodeExamples_ReturnsWhetherPermutationExists(
        string s1, string s2, bool expected) =>
        Assert.Equal(expected, PermutationInStringSolution.CheckInclusionByPerWindowRebuild(s1, s2));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckInclusionBySlidingWindow_LeetCodeExamples_ReturnsWhetherPermutationExists(
        string s1, string s2, bool expected) =>
        Assert.Equal(expected, PermutationInStringSolution.CheckInclusionBySlidingWindow(s1, s2));
}
