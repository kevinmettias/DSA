using DSAExperimentation.LeetCode.Solutions.LongestChunkedPalindromeDecomposition;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestChunkedPalindromeDecomposition;

// Harness only: both strategies live in
// LongestChunkedPalindromeDecompositionSolution and answer the same cases -
// LeetCode's three published cases, plus the degenerate lengths the two-pointer
// walk's tail condition turns on (a single character, an even-length pair that
// matches, an even-length pair that does not, an odd-length run, and a text whose
// whole decomposition is a single repeated pair).
public sealed partial class LongestChunkedPalindromeDecompositionSolutionTests
{
    public static TheoryData<string, int> Cases =>
        new()
        {
            { "aa", 2 },
            { "ab", 1 },
            { "aba", 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestDecompositionByRollingHash_Cases_ReturnsMaxChunkCount(string text, int expected) =>
        Assert.Equal(
            expected,
            LongestChunkedPalindromeDecompositionSolution.LongestDecompositionByRollingHash(text));

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestDecompositionByStringConcatenation_Cases_ReturnsMaxChunkCount(
        string text,
        int expected) =>
        Assert.Equal(
            expected,
            LongestChunkedPalindromeDecompositionSolution.LongestDecompositionByStringConcatenation(text));
}
