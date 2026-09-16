using DSAExperimentation.LeetCode.TrappingRainWater;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TrappingRainWater;

// Harness only. Both strategies live in TrappingRainWaterSolution - this file
// just pins them to LeetCode's published examples plus a couple of edge cases
// (no walls tall enough to trap anything, and a single bar).
public sealed partial class TrappingRainWaterTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [0, 1, 0, 2, 1, 0, 1, 3, 2, 1, 2, 1], 6 },
            { [4, 2, 0, 3, 2, 5], 9 },
            { [1, 2, 3, 4, 5], 0 },
            { [5], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TrapByBruteForce_LeetCodeExamples_ReturnsTotalTrappedWater(int[] height, int expected) =>
        Assert.Equal(expected, TrappingRainWaterSolution.TrapByBruteForce(height));

    [Theory]
    [MemberData(nameof(Examples))]
    public void TrapByMonotonicStack_LeetCodeExamples_ReturnsTotalTrappedWater(int[] height, int expected) =>
        Assert.Equal(expected, TrappingRainWaterSolution.TrapByMonotonicStack(height));
}
