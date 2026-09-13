using DSAExperimentation.LeetCode.CreateSortedArrayThroughInstructions;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CreateSortedArrayThroughInstructions;

// Harness only. Both strategies are CreateSortedArrayThroughInstructionsSolution's -
// this file just pins them to LeetCode's published examples, plus the degenerate
// orderings (already sorted either way, all equal, one element) where every
// insertion is free.
public sealed class CreateSortedArrayThroughInstructionsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 5, 6, 2], 1 },
            { [1, 2, 3, 6, 5, 4], 3 },
            { [1, 3, 3, 3, 2, 4, 2, 1, 2], 4 },
            { [1, 2, 3, 4, 5], 0 },
            { [5, 4, 3, 2, 1], 0 },
            { [2, 2, 2, 2], 0 },
            { [7], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateSortedArrayByPairwiseScan_LeetCodeExamples_ReturnsMinimumTotalInsertionCost(
        int[] instructions, int expected) =>
        Assert.Equal(
            expected,
            CreateSortedArrayThroughInstructionsSolution.CreateSortedArrayByPairwiseScan(instructions));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateSortedArrayByFenwickTreeSweep_LeetCodeExamples_ReturnsMinimumTotalInsertionCost(
        int[] instructions, int expected) =>
        Assert.Equal(
            expected,
            CreateSortedArrayThroughInstructionsSolution.CreateSortedArrayByFenwickTreeSweep(instructions));
}
