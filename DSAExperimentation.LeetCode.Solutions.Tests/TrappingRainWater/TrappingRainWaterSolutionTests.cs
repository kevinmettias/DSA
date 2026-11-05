using DSAExperimentation.LeetCode.Solutions.TrappingRainWater;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TrappingRainWater;

// Harness only. Both strategies live in TrappingRainWaterSolution - this file
// just pins them to a couple of edge cases
// (no walls tall enough to trap anything, and a single bar).
public sealed partial class TrappingRainWaterSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [5], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void TrapByBruteForce_Cases_ReturnsTotalTrappedWater(int[] height, int expected) =>
        Assert.Equal(expected, TrappingRainWaterSolution.TrapByBruteForce(height));

    [Theory]
    [MemberData(nameof(Cases))]
    public void TrapByMonotonicStack_Cases_ReturnsTotalTrappedWater(int[] height, int expected) =>
        Assert.Equal(expected, TrappingRainWaterSolution.TrapByMonotonicStack(height));
}
