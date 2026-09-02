using DSAExperimentation.LeetCode.LexicographicallySmallestStringAfterDeletingDuplicateCharacters;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LexicographicallySmallestStringAfterDeletingDuplicateCharacters;

// Harness only: both strategies live in
// LexicographicallySmallestStringAfterDeletingDuplicateCharactersSolution and are
// asserted against the same examples, including the single-character case where
// no deletion is even possible and a case with a trailing character that only a
// final cleanup pass (not the main left-to-right sweep) can drop.
public sealed class LexicographicallySmallestStringAfterDeletingDuplicateCharactersTests
{
    public static TheoryData<string, string> Examples =>
        new()
        {
            { "aaccb", "aacb" },
            { "z", "z" },
            { "abb", "ab" },
            { "bcabc", "abc" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestStringByRepeatedScan_LeetCodeExamples_ReturnsLexicographicallySmallestReachableString(
        string s, string expected) =>
        Assert.Equal(
            expected,
            LexicographicallySmallestStringAfterDeletingDuplicateCharactersSolution.SmallestStringByRepeatedScan(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestStringByMonotonicStack_LeetCodeExamples_ReturnsLexicographicallySmallestReachableString(
        string s, string expected) =>
        Assert.Equal(
            expected,
            LexicographicallySmallestStringAfterDeletingDuplicateCharactersSolution.SmallestStringByMonotonicStack(s));
}
