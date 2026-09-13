using DSAExperimentation.LeetCode.NumberOfMatchingSubsequences;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfMatchingSubsequences;

// Harness only. Both strategies - the per-word two-pointer baseline and the
// HashMap/Queue waiting-bucket pass - are NumberOfMatchingSubsequencesSolution's;
// this file pins them to LeetCode's published examples plus the cases the bucket
// pass has to get right on its own: words that share a waiting character, repeated
// words, and a word whose characters all appear but out of order.
public sealed class NumberOfMatchingSubsequencesTests
{
    public static TheoryData<string, string[], int> Examples =>
        new()
        {
            // LC example 1: "bb" is the only word that is not a subsequence.
            { "abcde", ["a", "bb", "acd", "ace"], 3 },

            // LC example 2.
            { "dsahjpjauf", ["ahjpjau", "ja", "ahbwzgqnuk", "tnmlanowax"], 2 },

            // Right characters, wrong order - and a character s never contains.
            { "abc", ["xyz", "ba"], 0 },

            // The same word twice counts twice.
            { "abcde", ["a", "a"], 2 },

            // Every word waits on the same character, and only some of its later
            // occurrences release them.
            { "aaab", ["aaa", "aab", "aaab", "aaaa"], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumMatchingSubseqByTwoPointerPerWord_LeetCodeExamples_CountsMatchingWords(
        string s, string[] words, int expected) =>
        Assert.Equal(
            expected, NumberOfMatchingSubsequencesSolution.NumMatchingSubseqByTwoPointerPerWord(s, words));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumMatchingSubseqByWaitingBuckets_LeetCodeExamples_CountsMatchingWords(
        string s, string[] words, int expected) =>
        Assert.Equal(
            expected, NumberOfMatchingSubsequencesSolution.NumMatchingSubseqByWaitingBuckets(s, words));
}
