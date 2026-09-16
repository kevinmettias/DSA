using DSAExperimentation.LeetCode.TallestBillboard;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TallestBillboard;

// Harness only: both strategies live in TallestBillboardSolution and are pinned to
// LeetCode's published examples, plus the cases the un-memoized baseline has to agree
// with the memoized arm on - a two-rod split, and a rod set whose powers of two admit no
// equal split at all. Rod counts stay small because the baseline really is 3^N.
public sealed partial class TallestBillboardTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 6], 6 },
            { [1, 2, 3, 4, 5, 6], 10 },
            { [1, 2], 0 },
            { [1, 1], 1 },
            { [2, 4, 8, 16], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxHeightByUnmemoizedRecursion_LeetCodeExamples_ReturnsTallestEqualSplit(
        int[] rods, int expected) =>
        Assert.Equal(expected, TallestBillboardSolution.MaxHeightByUnmemoizedRecursion(rods));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxHeightByMemoizedDiff_LeetCodeExamples_ReturnsTallestEqualSplit(
        int[] rods, int expected) =>
        Assert.Equal(expected, TallestBillboardSolution.MaxHeightByMemoizedDiff(rods));
}
