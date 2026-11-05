using DSAExperimentation.LeetCode.Solutions.SumOfPrefixScoresOfStrings;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SumOfPrefixScoresOfStrings;

// Harness only: both strategies live in SumOfPrefixScoresOfStringsSolution and are
// asserted against the same cases, among them the duplicate-word case where one
// word's own prefixes are counted twice and the nested case where every word is a
// prefix of the next.
public sealed partial class SumOfPrefixScoresOfStringsSolutionTests
{
    public static TheoryData<string[], int[]> Cases =>
        new()
        {
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
    [MemberData(nameof(Cases))]
    public void SumPrefixScoresByStartsWithScan_Cases_ReturnsPrefixScoreSums(
        string[] words, int[] expected) =>
        Assert.Equal(expected, SumOfPrefixScoresOfStringsSolution.SumPrefixScoresByStartsWithScan(words));

    [Theory]
    [MemberData(nameof(Cases))]
    public void SumPrefixScoresByPrefixCountingTrie_Cases_ReturnsPrefixScoreSums(
        string[] words, int[] expected) =>
        Assert.Equal(expected, SumOfPrefixScoresOfStringsSolution.SumPrefixScoresByPrefixCountingTrie(words));
}
