using DSAExperimentation.LeetCode.Solutions.MoveZeroes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MoveZeroes;

// Harness only. Both strategies are MoveZeroesSolution's - this file just pins
// them to an array with no zeroes, which both must leave as it was. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class MoveZeroesSolutionTests
{
    public static TheoryData<int[], int[]> Cases =>
        new()
        {
            { [4, 2, 7], [4, 2, 7] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MoveZeroesToEndByLinearScan_Cases_MovesZeroesToEndPreservingOrder(
        int[] nums, int[] expected)
    {
        var copy = (int[])nums.Clone();

        MoveZeroesSolution.MoveZeroesToEndByLinearScan(copy);

        Assert.Equal(expected, copy);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MoveZeroesToEndByArrayIndexedTwoPointer_Cases_MovesZeroesToEndPreservingOrder(
        int[] nums, int[] expected)
    {
        var copy = (int[])nums.Clone();

        MoveZeroesSolution.MoveZeroesToEndByArrayIndexedTwoPointer(copy);

        Assert.Equal(expected, copy);
    }
}
