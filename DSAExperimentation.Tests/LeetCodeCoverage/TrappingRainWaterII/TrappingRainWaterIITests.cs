using DSAExperimentation.LeetCode.TrappingRainWaterII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TrappingRainWaterII;

// Harness only. Both search strategies are TrappingRainWaterIISolution's -
// this file just pins them to LeetCode's published examples, plus a
// smaller-than-3x3 case neither original arm exercised.
public sealed class TrappingRainWaterIITests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            {
                [
                    [1, 4, 3, 1, 3, 2],
                    [3, 2, 1, 3, 2, 4],
                    [2, 3, 3, 2, 3, 1],
                ],
                4
            },
            {
                [
                    [3, 3, 3, 3, 3],
                    [3, 2, 2, 2, 3],
                    [3, 2, 1, 2, 3],
                    [3, 2, 2, 2, 3],
                    [3, 3, 3, 3, 3],
                ],
                10
            },
            {
                [
                    [1, 2],
                    [3, 4],
                ],
                0
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TrapRainWaterByRelaxationSweep_LeetCodeExamples_ReturnsTotalTrappedVolume(int[][] heightMap, int expected) =>
        Assert.Equal(expected, TrappingRainWaterIISolution.TrapRainWaterByRelaxationSweep(heightMap));

    [Theory]
    [MemberData(nameof(Examples))]
    public void TrapRainWaterByHeapFloodFill_LeetCodeExamples_ReturnsTotalTrappedVolume(int[][] heightMap, int expected) =>
        Assert.Equal(expected, TrappingRainWaterIISolution.TrapRainWaterByHeapFloodFill(heightMap));
}
