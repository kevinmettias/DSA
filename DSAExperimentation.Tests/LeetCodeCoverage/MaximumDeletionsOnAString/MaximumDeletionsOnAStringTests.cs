using DSAExperimentation.LeetCode.MaximumDeletionsOnAString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumDeletionsOnAString;

// Harness only. Both deletion-count strategies are MaximumDeletionsOnAStringSolution's
// - the naive substring comparison that used to live untested as the benchmark
// baseline, and the RollingHash-screened DP - pinned here to LeetCode's published
// examples plus the cases that separate "take the smallest valid deletion" from
// "take the best one" ("aaabaab", where deleting "aab" reaches 4 operations and
// deleting "a" only reaches 3).
public sealed class MaximumDeletionsOnAStringTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "abcabcdabc", 2 },
            { "aaabaab", 4 },
            { "aaaaa", 5 },
            { "abcabcabc", 3 },
            { "a", 1 },
            { "aa", 2 },
            { "ab", 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxOperationsByNaiveSubstringComparison_LeetCodeExamples_ReturnsExpectedOperationCount(
        string text, int expected) =>
        Assert.Equal(expected, MaximumDeletionsOnAStringSolution.MaxOperationsByNaiveSubstringComparison(text));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxOperationsByRollingHashScreen_LeetCodeExamples_ReturnsExpectedOperationCount(
        string text, int expected) =>
        Assert.Equal(expected, MaximumDeletionsOnAStringSolution.MaxOperationsByRollingHashScreen(text));
}
