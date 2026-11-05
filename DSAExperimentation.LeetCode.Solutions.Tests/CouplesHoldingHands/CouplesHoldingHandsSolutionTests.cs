using DSAExperimentation.LeetCode.Solutions.CouplesHoldingHands;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CouplesHoldingHands;

// Harness only. Both strategies are CouplesHoldingHandsSolution's - this file just
// pins them to an already-paired edge case.
public sealed partial class CouplesHoldingHandsSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            // Couple ids by seat: 2,0,1,2,0,1 - a single 3-couple cycle, needing 2 swaps.
            { [4, 0, 2, 5, 1, 3], 2 },
            { [0, 1], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinSwapsByGreedySwap_Cases_ReturnsMinimumSwapCount(int[] row, int expected) =>
        Assert.Equal(expected, CouplesHoldingHandsSolution.MinSwapsByGreedySwap(row));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinSwapsByDisjointSet_Cases_ReturnsMinimumSwapCount(int[] row, int expected) =>
        Assert.Equal(expected, CouplesHoldingHandsSolution.MinSwapsByDisjointSet(row));
}
