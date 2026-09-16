using DSAExperimentation.LeetCode.NumberOfMatchingSubsequences;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfMatchingSubsequences;

// Harness only. Both strategies - the per-word two-pointer baseline and the
// HashMap/Queue waiting-bucket pass - are NumberOfMatchingSubsequencesSolution's;
// this file pins them to LeetCode's published examples plus the cases the bucket
// pass has to get right on its own: words that share a waiting character, repeated
// words, and a word whose characters all appear but out of order.
public sealed partial class NumberOfMatchingSubsequencesTests
{
    public static TheoryData<string, string[], int> Examples =>
        new()
        {
            // LC example 1: "bb" is the only word that is not a subsequence.
            { "abcde", ["a", "bb", "acd", "ace"], 3 },

            // LC example 2.
            { "dsahjpjauf", ["ahjpjau", "ja", "ahbwzgqnuk", "tnmlanowax"], 2 },

            // Right characters, wrong order - and a character searchedText never contains.
            { "abc", ["xyz", "ba"], 0 },

            // The same word twice counts twice.
            { "abcde", ["a", "a"], 2 },

            // Every word waits on the same character, and only some of its later
            // occurrences release them.
            { "aaab", ["aaa", "aab", "aaab", "aaaa"], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountMatchingSubseqByTwoPointerPerWord_LeetCodeExamples_CountsMatchingWords(
        string searchedText, string[] words, int expected)
    {
        var actual = NumberOfMatchingSubsequencesSolution.CountMatchingSubseqByTwoPointerPerWord(searchedText, words);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountMatchingSubseqByWaitingBuckets_LeetCodeExamples_CountsMatchingWords(
        string searchedText, string[] words, int expected)
    {
        var actual = NumberOfMatchingSubsequencesSolution.CountMatchingSubseqByWaitingBuckets(searchedText, words);

        Assert.Equal(expected, actual);
    }
}
