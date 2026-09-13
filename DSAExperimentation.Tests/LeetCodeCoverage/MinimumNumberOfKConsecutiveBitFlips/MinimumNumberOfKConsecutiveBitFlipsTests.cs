using DSAExperimentation.LeetCode.MinimumNumberOfKConsecutiveBitFlips;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfKConsecutiveBitFlips;

// Harness only. Both strategies are MinimumNumberOfKConsecutiveBitFlipsSolution's;
// this file pins them to LeetCode's published examples plus the boundary cases the
// two arms disagree about most easily - an already-all-ones input, a flip that
// exactly fills the array, and a window wider than the array itself.
public sealed class MinimumNumberOfKConsecutiveBitFlipsTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [0, 1, 0], 1, 2 },
            { [1, 1, 0], 2, -1 },
            { [0, 0, 0, 1, 0, 1, 1, 0], 3, 3 },
            { [1, 1, 1], 2, 0 },
            { [0, 0], 2, 1 },
            { [0], 2, -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinKBitFlipsByInPlaceWindowFlip_LeetCodeExamples_ReturnsMinimumFlipCount(
        int[] nums, int k, int expected) =>
        Assert.Equal(expected, MinimumNumberOfKConsecutiveBitFlipsSolution.MinKBitFlipsByInPlaceWindowFlip(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinKBitFlipsByQueueTrackedParity_LeetCodeExamples_ReturnsMinimumFlipCount(
        int[] nums, int k, int expected) =>
        Assert.Equal(expected, MinimumNumberOfKConsecutiveBitFlipsSolution.MinKBitFlipsByQueueTrackedParity(nums, k));
}
