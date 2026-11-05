using DSAExperimentation.LeetCode.Solutions.SumOfScoresOfBuiltStrings;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SumOfScoresOfBuiltStrings;

// Harness only: both strategies live in SumOfScoresOfBuiltStringsSolution,
// including the suffix-comparison baseline the pre-migration benchmark kept to
// itself.
//
// The cases are an all-one-letter string, which reaches the maximum score, a
// palindromic overlap, and "aab", the case that would have caught the reversed-Z bug
// the pre-migration test carried (see the solution's own note): every input the old
// test asserted is either a palindrome or free of self-overlap, and on exactly
// those inputs summing the Z-array of reverse(s) accidentally agrees with the
// answer. "aab" and "banana" are the smallest inputs where it does not; "banana" is
// one of the catalog's cases. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class SumOfScoresOfBuiltStringsSolutionTests
{
    // (text, sum of every suffix's longest common prefix with text)
    public static TheoryData<string, long> Cases =>
        new()
        {
            // Every suffix of an all-one-letter string is a prefix of s, so the
            // scores are 1 + 2 + 3 + 4 - the maximum a length-4 string can reach.
            { "aaaa", 10 },

            // Palindromic overlap: scores 1, 2, 0, 1, 5.
            { "aabaa", 9 },

            // Scores 0, 1, 3. Reversed-Z reports 3 here.
            { "aab", 4 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SumScoresBySuffixComparison_Cases_ReturnsTotalScore(string text, long expected) =>
        Assert.Equal(expected, SumOfScoresOfBuiltStringsSolution.SumScoresBySuffixComparison(text));

    [Theory]
    [MemberData(nameof(Cases))]
    public void SumScoresByZFunction_Cases_ReturnsTotalScore(string text, long expected) =>
        Assert.Equal(expected, SumOfScoresOfBuiltStringsSolution.SumScoresByZFunction(text));
}
