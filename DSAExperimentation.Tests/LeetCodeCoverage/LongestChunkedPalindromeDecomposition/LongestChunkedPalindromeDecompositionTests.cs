using DSAExperimentation.LeetCode.LongestChunkedPalindromeDecomposition;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestChunkedPalindromeDecomposition;

// Harness only: both strategies live in
// LongestChunkedPalindromeDecompositionSolution and answer the same examples -
// LeetCode's three published cases, plus the degenerate lengths the two-pointer
// walk's tail condition turns on (a single character, an even-length pair that
// matches, an even-length pair that does not, an odd-length run, and a text whose
// whole decomposition is a single repeated pair).
public sealed partial class LongestChunkedPalindromeDecompositionTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "ghiabcdefhelloadamhelloabcdefghi", 7 },
            { "merchant", 1 },
            { "antaprezatepzapreanta", 11 },
            { "a", 1 },
            { "aa", 2 },
            { "ab", 1 },
            { "aba", 3 },
            { "aaa", 3 },
            { "abab", 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestDecompositionByRollingHash_LeetCodeExamples_ReturnsMaxChunkCount(string text, int expected) =>
        Assert.Equal(
            expected,
            LongestChunkedPalindromeDecompositionSolution.LongestDecompositionByRollingHash(text));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestDecompositionByStringConcatenation_LeetCodeExamples_ReturnsMaxChunkCount(
        string text,
        int expected) =>
        Assert.Equal(
            expected,
            LongestChunkedPalindromeDecompositionSolution.LongestDecompositionByStringConcatenation(text));
}
