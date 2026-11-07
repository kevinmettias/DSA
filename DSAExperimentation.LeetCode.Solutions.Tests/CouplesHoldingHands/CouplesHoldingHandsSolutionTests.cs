using DSAExperimentation.LeetCode.Solutions.CouplesHoldingHands;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CouplesHoldingHands;

// Harness only. Every strategy is CouplesHoldingHandsSolution's - this file just
// pins them to an already-paired edge case.
public sealed partial class CouplesHoldingHandsSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            // Couple ids by seat: 2,0,1,2,0,1 - a single 3-couple cycle, needing 2 swaps.
            { [4, 0, 2, 5, 1, 3], 2 },
            { [0, 1], 0 },
            // Couple ids by seat: 0,1,1,0 - two couples in one cycle, 1 swap. This seating is
            // not its own inverse, so a composite using it where its inverse belongs shows.
            { [1, 2, 3, 0], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinSwapsByGreedySwap_Cases_ReturnsMinimumSwapCount(int[] row, int expected) =>
        Assert.Equal(expected, CouplesHoldingHandsSolution.MinSwapsByGreedySwap(row));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinSwapsByDisjointSet_Cases_ReturnsMinimumSwapCount(int[] row, int expected) =>
        Assert.Equal(expected, CouplesHoldingHandsSolution.MinSwapsByDisjointSet(row));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinSwapsByPermutationCycles_Cases_ReturnsMinimumSwapCount(
        int[] row, int expected) =>
        Assert.Equal(expected, CouplesHoldingHandsSolution.MinSwapsByPermutationCycles(row));
}
