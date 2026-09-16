using DSAExperimentation.LeetCode.LexicographicallySmallestStringAfterDeletingDuplicateCharacters;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LexicographicallySmallestStringAfterDeletingDuplicateCharacters;

// Harness only: both strategies live in
// LexicographicallySmallestStringAfterDeletingDuplicateCharactersSolution and are
// asserted against the same examples, including the single-character case where
// no deletion is even possible and a case with a trailing character that only a
// final cleanup pass (not the main left-to-right sweep) can drop.
public sealed class LexicographicallySmallestStringAfterDeletingDuplicateCharactersTests
{
    public static TheoryData<DeletionExample> Examples =>
        new()
        {
            { new DeletionExample(S: "aaccb", Expected: "aacb") },
            { new DeletionExample(S: "z", Expected: "z") },
            { new DeletionExample(S: "abb", Expected: "ab") },
            { new DeletionExample(S: "bcabc", Expected: "abc") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestStringByRepeatedScan_LeetCodeExamples_ReturnsLexicographicallySmallestReachableString(
        DeletionExample example)
    {
        var actual =
            LexicographicallySmallestStringAfterDeletingDuplicateCharactersSolution.SmallestStringByRepeatedScan(
                example.S);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestStringByMonotonicStack_LeetCodeExamples_ReturnsLexicographicallySmallestReachableString(
        DeletionExample example)
    {
        var actual =
            LexicographicallySmallestStringAfterDeletingDuplicateCharactersSolution.SmallestStringByMonotonicStack(
                example.S);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the string to delete duplicate characters from, and the
    // lexicographically smallest result. Both are `string` and the question is not
    // symmetric - the result is derived from the input, never the other way round - so
    // the row names the roles instead of leaving two adjacent positions a caller could
    // swap with the compiler silent.
    public readonly record struct DeletionExample(string S, string Expected);
}
