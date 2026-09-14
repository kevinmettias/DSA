using DSAExperimentation.LeetCode.SumOfPrefixScoresOfStrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfPrefixScoresOfStrings;

// Harness only: both strategies live in SumOfPrefixScoresOfStringsSolution and are
// asserted against the same examples, including the duplicate-word case where one
// word's own prefixes are counted twice and the nested case where every word is a
// prefix of the next.
public sealed class SumOfPrefixScoresOfStringsTests
{
    public static TheoryData<string[], int[]> Examples =>
        new()
        {
            // LeetCode example 1.
            { ["abc", "ab", "bc", "b"], [5, 4, 3, 2] },

            // LeetCode example 2: a single word scores one per prefix of itself.
            { ["abcd"], [4] },

            // Nothing shared, so each word scores only its own prefixes.
            { ["a", "b", "c"], [1, 1, 1] },

            // A chain where each word is a prefix of the next: the shortest prefix
            // is shared by all three, the longest by one.
            { ["a", "ab", "abc"], [3, 5, 6] },

            // The same word twice: both copies count towards each other's prefixes,
            // so both score double a lone occurrence.
            { ["ab", "ab"], [4, 4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumPrefixScoresByStartsWithScan_LeetCodeExamples_ReturnsPrefixScoreSums(
        string[] words, int[] expected) =>
        Assert.Equal(expected, SumOfPrefixScoresOfStringsSolution.SumPrefixScoresByStartsWithScan(words));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumPrefixScoresByPrefixCountingTrie_LeetCodeExamples_ReturnsPrefixScoreSums(
        string[] words, int[] expected) =>
        Assert.Equal(expected, SumOfPrefixScoresOfStringsSolution.SumPrefixScoresByPrefixCountingTrie(words));
}
