using DSAExperimentation.LeetCode.Solutions.MaximumDeletionsOnAString;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumDeletionsOnAString;

// Harness only. Both deletion-count strategies are MaximumDeletionsOnAStringSolution's
// - the naive substring comparison that used to live untested as the benchmark
// baseline, and the RollingHash-screened DP - pinned here to one block repeated three
// times and to the shortest strings: one character, a doubled character, and two
// different characters, where no prefix repeats and the whole string goes in one
// operation. The case that separates "take the smallest valid deletion" from "take
// the best one" is LeetCode's "aaabaab", where deleting "aab" reaches 4 operations
// and deleting "a" only reaches 3. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class MaximumDeletionsOnAStringSolutionTests
{
    public static TheoryData<string, int> Cases =>
        new()
        {
            { "abcabcabc", 3 },
            { "a", 1 },
            { "aa", 2 },
            { "ab", 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxOperationsByNaiveSubstringComparison_Cases_ReturnsExpectedOperationCount(
        string text, int expected) =>
        Assert.Equal(expected, MaximumDeletionsOnAStringSolution.MaxOperationsByNaiveSubstringComparison(text));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxOperationsByRollingHashScreen_Cases_ReturnsExpectedOperationCount(
        string text, int expected) =>
        Assert.Equal(expected, MaximumDeletionsOnAStringSolution.MaxOperationsByRollingHashScreen(text));
}
