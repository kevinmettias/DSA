using DSAExperimentation.LeetCode.FindTheShortestSuperstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheShortestSuperstring;

// Harness only. Both the overlap matrix and the two orderings searches are
// FindTheShortestSuperstringSolution's; this file pins them to LeetCode's published
// examples plus the degenerate shapes (one word, two words, a mutual one-character
// overlap) the original test never exercised.
//
// The assertion is the answer's length plus containment of every word rather than an
// exact string, because LC 943 accepts any superstring of the minimum length and the
// two strategies legitimately break ties differently.
public sealed class FindTheShortestSuperstringTests
{
    public static TheoryData<string[], int> Examples =>
        new()
        {
            { ["alex", "loves", "leetcode"], 17 },
            { ["catg", "ctaagt", "gcta", "ttca", "atgcatc"], 16 },
            { ["abc"], 3 },
            { ["abcd", "cdef"], 6 },
            { ["ab", "ba"], 3 },
            { ["aaa", "aab", "baa"], 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestSuperstringByPermutations_LeetCodeExamples_ReturnsShortestStringContainingEveryWord(
        string[] words, int expectedLength) =>
        AssertShortestSuperstring(
            FindTheShortestSuperstringSolution.ShortestSuperstringByPermutations(words), words, expectedLength);

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestSuperstringByMemoizedBitmask_LeetCodeExamples_ReturnsShortestStringContainingEveryWord(
        string[] words, int expectedLength) =>
        AssertShortestSuperstring(
            FindTheShortestSuperstringSolution.ShortestSuperstringByMemoizedBitmask(words), words, expectedLength);

    private static void AssertShortestSuperstring(string actual, string[] words, int expectedLength)
    {
        Assert.Equal(expectedLength, actual.Length);
        Assert.All(words, word => Assert.Contains(word, actual));
    }
}
