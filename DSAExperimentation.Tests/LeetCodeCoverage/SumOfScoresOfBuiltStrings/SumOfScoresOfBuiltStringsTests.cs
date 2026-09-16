using DSAExperimentation.LeetCode.SumOfScoresOfBuiltStrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfScoresOfBuiltStrings;

// Harness only: both strategies live in SumOfScoresOfBuiltStringsSolution,
// including the suffix-comparison baseline the pre-migration benchmark kept to
// itself.
//
// The last three cases are the ones that would have caught the reversed-Z bug the
// pre-migration test carried (see the solution's own note): every example the old
// test asserted is either a palindrome or free of self-overlap, and on exactly
// those inputs summing the Z-array of reverse(s) accidentally agrees with the
// answer. "aab" and "banana" are the smallest inputs where it does not.
public sealed partial class SumOfScoresOfBuiltStringsTests
{
    // (text, sum of every suffix's longest common prefix with text)
    public static TheoryData<string, long> Examples =>
        new()
        {
            // LeetCode example 1: scores are 1, 0, 3, 0, 5.
            { "babab", 9 },

            // LeetCode example 2.
            { "azbazbzaz", 14 },

            // A single character scores itself and nothing else.
            { "a", 1 },

            // No suffix shares a first character with s, so only t_n contributes.
            { "abcde", 5 },

            // Every suffix of an all-one-letter string is a prefix of s, so the
            // scores are 1 + 2 + 3 + 4 - the maximum a length-4 string can reach.
            { "aaaa", 10 },

            // Overlapping repeats: scores 0, 0, 3, 0, 0, 6, 0, 0, 9.
            { "abcabcabc", 18 },

            // Palindromic overlap: scores 1, 2, 0, 1, 5.
            { "aabaa", 9 },

            // Scores 0, 1, 3. Reversed-Z reports 3 here.
            { "aab", 4 },

            // Only t_n scores at all. Reversed-Z reports 10 here.
            { "banana", 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumScoresBySuffixComparison_LeetCodeExamples_ReturnsTotalScore(string text, long expected) =>
        Assert.Equal(expected, SumOfScoresOfBuiltStringsSolution.SumScoresBySuffixComparison(text));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumScoresByZFunction_LeetCodeExamples_ReturnsTotalScore(string text, long expected) =>
        Assert.Equal(expected, SumOfScoresOfBuiltStringsSolution.SumScoresByZFunction(text));
}
