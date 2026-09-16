using DSAExperimentation.LeetCode.SpiralMatrixII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SpiralMatrixII;

// Harness only: both strategies live in SpiralMatrixIISolution and are asserted
// against the same examples, so a failure names the strategy that broke.
public sealed partial class SpiralMatrixIITests
{
    public static TheoryData<int, int[][]> Examples =>
        new()
        {
            { 1, [[1]] },
            { 2, [[1, 2], [4, 3]] },
            { 3, [[1, 2, 3], [8, 9, 4], [7, 6, 5]] },
            { 4, [[1, 2, 3, 4], [12, 13, 14, 5], [11, 16, 15, 6], [10, 9, 8, 7]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GenerateMatrixByDirectionVectorWalk_LeetCodeExamples_FillsBoundsClockwise(int size, int[][] expected) =>
        Assert.Equal(expected, SpiralMatrixIISolution.GenerateMatrixByDirectionVectorWalk(size));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GenerateMatrixByBoundaryShrinking_LeetCodeExamples_FillsBoundsClockwise(int size, int[][] expected) =>
        Assert.Equal(expected, SpiralMatrixIISolution.GenerateMatrixByBoundaryShrinking(size));
}
