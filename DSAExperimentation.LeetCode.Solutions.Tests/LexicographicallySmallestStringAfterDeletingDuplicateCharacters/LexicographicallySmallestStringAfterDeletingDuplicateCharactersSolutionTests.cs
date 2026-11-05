using DSAExperimentation.LeetCode.Solutions.LexicographicallySmallestStringAfterDeletingDuplicateCharacters;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LexicographicallySmallestStringAfterDeletingDuplicateCharacters;

// Harness only: both strategies live in
// LexicographicallySmallestStringAfterDeletingDuplicateCharactersSolution and are
// asserted against the same cases, among them the single-character case where
// no deletion is even possible and a case with a trailing character that only a
// final cleanup pass (not the main left-to-right sweep) can drop.
public sealed partial class LexicographicallySmallestStringAfterDeletingDuplicateCharactersSolutionTests
{
    public static TheoryData<DeletionCase> Cases =>
        new()
        {
            { new DeletionCase(S: "abb", Expected: "ab") },
            { new DeletionCase(S: "bcabc", Expected: "abc") },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SmallestStringByRepeatedScan_Cases_ReturnsLexicographicallySmallestReachableString(
        DeletionCase testCase)
    {
        var actual =
            LexicographicallySmallestStringAfterDeletingDuplicateCharactersSolution.SmallestStringByRepeatedScan(
                testCase.S);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SmallestStringByMonotonicStack_Cases_ReturnsLexicographicallySmallestReachableString(
        DeletionCase testCase)
    {
        var actual =
            LexicographicallySmallestStringAfterDeletingDuplicateCharactersSolution.SmallestStringByMonotonicStack(
                testCase.S);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the string to delete duplicate characters from, and the
    // lexicographically smallest result. Both are `string` and the question is not
    // symmetric - the result is derived from the input, never the other way round - so
    // the row names the roles instead of leaving two adjacent positions a caller could
    // swap with the compiler silent.
    public readonly record struct DeletionCase(string S, string Expected);
}
