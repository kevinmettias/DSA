using DSAExperimentation.LeetCode.CountingBits;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountingBits;

// Harness only. Both strategies live in CountingBitsSolution - this file just pins
// them to LeetCode's published examples (plus n = 0, the constraint's lower bound).
public sealed class CountingBitsTests
{
    public static TheoryData<int, int[]> Examples =>
        new()
        {
            { 0, new[] { 0 } },
            { 2, new[] { 0, 1, 1 } },
            { 5, new[] { 0, 1, 1, 2, 1, 2 } },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountBitsByPerNumberLoop_LeetCodeExamples_ReturnsBitCountsForEveryIndex(
        int n, int[] expected) =>
        Assert.Equal(expected, CountingBitsSolution.CountBitsByPerNumberLoop(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountBitsByMemoizedRecurrence_LeetCodeExamples_ReturnsBitCountsForEveryIndex(
        int n, int[] expected) =>
        Assert.Equal(expected, CountingBitsSolution.CountBitsByMemoizedRecurrence(n));
}
