using DSAExperimentation.LeetCode.MinimumNumberOfMovesToMakePalindrome;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfMovesToMakePalindrome;

// Harness only. Both buffer strategies are
// MinimumNumberOfMovesToMakePalindromeSolution's; this file pins them to
// LeetCode's published examples plus an already-palindromic input and the
// three-character odd-length case, which is the smallest input that exercises the
// lone-middle-character nudge.
public sealed partial class MinimumNumberOfMovesToMakePalindromeTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "aabb", 2 },
            { "letelt", 2 },
            { "cc", 0 },
            { "ababa", 0 },
            { "abb", 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMovesByListRemoveInsert_LeetCodeExamples_ReturnsMinimumAdjacentSwaps(string text, int expected) =>
        Assert.Equal(expected, MinimumNumberOfMovesToMakePalindromeSolution.MinMovesByListRemoveInsert(text));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMovesByIndexedSequenceSwap_LeetCodeExamples_ReturnsMinimumAdjacentSwaps(string text, int expected) =>
        Assert.Equal(expected, MinimumNumberOfMovesToMakePalindromeSolution.MinMovesByIndexedSequenceSwap(text));
}
