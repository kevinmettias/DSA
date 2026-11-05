using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfMovesToMakePalindrome;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfMovesToMakePalindrome;

// Harness only. Both buffer strategies are
// MinimumNumberOfMovesToMakePalindromeSolution's; this file pins them to
// an already-palindromic input and the
// three-character odd-length case, which is the smallest input that exercises the
// lone-middle-character nudge.
public sealed partial class MinimumNumberOfMovesToMakePalindromeSolutionTests
{
    public static TheoryData<string, int> Cases =>
        new()
        {
            { "cc", 0 },
            { "ababa", 0 },
            { "abb", 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinMovesByListRemoveInsert_Cases_ReturnsMinimumAdjacentSwaps(string text, int expected) =>
        Assert.Equal(expected, MinimumNumberOfMovesToMakePalindromeSolution.MinMovesByListRemoveInsert(text));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinMovesByIndexedSequenceSwap_Cases_ReturnsMinimumAdjacentSwaps(string text, int expected) =>
        Assert.Equal(expected, MinimumNumberOfMovesToMakePalindromeSolution.MinMovesByIndexedSequenceSwap(text));
}
