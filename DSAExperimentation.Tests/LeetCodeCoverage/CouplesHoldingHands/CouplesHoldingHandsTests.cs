using DSAExperimentation.LeetCode.CouplesHoldingHands;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CouplesHoldingHands;

// Harness only. Both strategies are CouplesHoldingHandsSolution's - this file just
// pins them to LeetCode's published examples plus an already-paired edge case.
public sealed partial class CouplesHoldingHandsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [0, 2, 1, 3], 1 },
            { [3, 2, 0, 1], 0 },
            // Couple ids by seat: 2,0,1,2,0,1 - a single 3-couple cycle, needing 2 swaps.
            { [4, 0, 2, 5, 1, 3], 2 },
            { [0, 1], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinSwapsByGreedySwap_LeetCodeExamples_ReturnsMinimumSwapCount(int[] row, int expected) =>
        Assert.Equal(expected, CouplesHoldingHandsSolution.MinSwapsByGreedySwap(row));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinSwapsByDisjointSet_LeetCodeExamples_ReturnsMinimumSwapCount(int[] row, int expected) =>
        Assert.Equal(expected, CouplesHoldingHandsSolution.MinSwapsByDisjointSet(row));
}
