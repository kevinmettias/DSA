using DSAExperimentation.LeetCode.SeparateSquaresII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SeparateSquaresII;

// Harness only. Both sweep strategies are SeparateSquaresIISolution's - this file
// just pins them to LeetCode's published examples. LC accepts answers within 1e-5,
// so assertions round to 5 decimal places rather than requiring exact equality.
public sealed partial class SeparateSquaresIITests
{
    public static TheoryData<int[][], double> Examples =>
        new()
        {
            { [[0, 0, 1], [2, 2, 1]], 1.0 },
            { [[0, 0, 2], [1, 1, 1]], 1.0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinYByEventSweep_LeetCodeExamples_ReturnsEqualSplitLine(int[][] squares, double expected) =>
        Assert.Equal(expected, SeparateSquaresIISolution.MinYByEventSweep(squares), precision: 5);

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinYByIntervalSetSweep_LeetCodeExamples_ReturnsEqualSplitLine(int[][] squares, double expected) =>
        Assert.Equal(expected, SeparateSquaresIISolution.MinYByIntervalSetSweep(squares), precision: 5);
}
