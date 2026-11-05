using DSAExperimentation.LeetCode.Solutions.CreateSortedArrayThroughInstructions;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CreateSortedArrayThroughInstructions;

// Harness only. Both strategies are CreateSortedArrayThroughInstructionsSolution's -
// this file just pins them to the degenerate
// orderings (already sorted either way, all equal, one element) where every
// insertion is free.
public sealed partial class CreateSortedArrayThroughInstructionsSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1, 2, 3, 4, 5], 0 },
            { [2, 2, 2, 2], 0 },
            { [7], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CreateSortedArrayByPairwiseScan_Cases_ReturnsMinimumTotalInsertionCost(
        int[] instructions, int expected) =>
        Assert.Equal(
            expected,
            CreateSortedArrayThroughInstructionsSolution.CreateSortedArrayByPairwiseScan(instructions));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CreateSortedArrayByFenwickTreeSweep_Cases_ReturnsMinimumTotalInsertionCost(
        int[] instructions, int expected) =>
        Assert.Equal(
            expected,
            CreateSortedArrayThroughInstructionsSolution.CreateSortedArrayByFenwickTreeSweep(instructions));
}
