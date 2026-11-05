using DSAExperimentation.LeetCode.Solutions.TallestBillboard;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TallestBillboard;

// Harness only: both strategies live in TallestBillboardSolution and are pinned to
// the cases the un-memoized baseline has to agree
// with the memoized arm on - a two-rod split, and a rod set whose powers of two admit no
// equal split at all. Rod counts stay small because the baseline really is 3^N.
public sealed partial class TallestBillboardSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1, 1], 1 },
            { [2, 4, 8, 16], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxHeightByUnmemoizedRecursion_Cases_ReturnsTallestEqualSplit(
        int[] rods, int expected) =>
        Assert.Equal(expected, TallestBillboardSolution.MaxHeightByUnmemoizedRecursion(rods));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxHeightByMemoizedDiff_Cases_ReturnsTallestEqualSplit(
        int[] rods, int expected) =>
        Assert.Equal(expected, TallestBillboardSolution.MaxHeightByMemoizedDiff(rods));
}
