using DSAExperimentation.LeetCode.Solutions.TrappingRainWaterII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TrappingRainWaterII;

// Harness only. Both search strategies are TrappingRainWaterIISolution's -
// this file just pins them to a
// smaller-than-3x3 case neither original arm exercised.
public sealed partial class TrappingRainWaterIISolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            {
                [
                    [1, 2],
                    [3, 4],
                ],
                0
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void TrapRainWaterByRelaxationSweep_Cases_ReturnsTotalTrappedVolume(int[][] heightMap, int expected) =>
        Assert.Equal(expected, TrappingRainWaterIISolution.TrapRainWaterByRelaxationSweep(heightMap));

    [Theory]
    [MemberData(nameof(Cases))]
    public void TrapRainWaterByHeapFloodFill_Cases_ReturnsTotalTrappedVolume(int[][] heightMap, int expected) =>
        Assert.Equal(expected, TrappingRainWaterIISolution.TrapRainWaterByHeapFloodFill(heightMap));
}
