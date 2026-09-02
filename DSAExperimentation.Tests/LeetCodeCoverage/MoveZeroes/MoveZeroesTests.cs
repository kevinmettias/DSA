using DSAExperimentation.LeetCode.MoveZeroes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MoveZeroes;

// Harness only. Both strategies are MoveZeroesSolution's - this file just pins
// them to LeetCode's published examples.
public sealed class MoveZeroesTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [0, 1, 0, 3, 12], [1, 3, 12, 0, 0] },
            { [0], [0] },
            { [4, 2, 7], [4, 2, 7] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MoveZeroesToEndByLinearScan_LeetCodeExamples_MovesZeroesToEndPreservingOrder(
        int[] nums, int[] expected)
    {
        var copy = (int[])nums.Clone();

        MoveZeroesSolution.MoveZeroesToEndByLinearScan(copy);

        Assert.Equal(expected, copy);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MoveZeroesToEndByArrayIndexedTwoPointer_LeetCodeExamples_MovesZeroesToEndPreservingOrder(
        int[] nums, int[] expected)
    {
        var copy = (int[])nums.Clone();

        MoveZeroesSolution.MoveZeroesToEndByArrayIndexedTwoPointer(copy);

        Assert.Equal(expected, copy);
    }
}
