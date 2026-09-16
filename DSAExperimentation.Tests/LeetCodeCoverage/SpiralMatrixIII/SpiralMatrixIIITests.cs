using DSAExperimentation.LeetCode.SpiralMatrixIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SpiralMatrixIII;

// Harness only: both strategies live in SpiralMatrixIIISolution and are asserted
// against the same examples, so a failure names the strategy that broke. The
// visited-set arm was previously only a benchmark baseline and nothing asserted
// it; it is under test here for the first time.
public sealed class SpiralMatrixIIITests
{
    public static TheoryData<SpiralExample> Examples =>
        new()
        {
            // LC example 1: one row, starting at its left end.
            new SpiralExample(1, 4, 0, 0, [[0, 0], [0, 1], [0, 2], [0, 3]]),

            // LC example 2: an interior start, so the spiral leaves and re-enters
            // the grid several times before the last corner is reached.
            new SpiralExample(
                5, 6, 1, 4,
                [
                    [1, 4], [1, 5], [2, 5], [2, 4], [2, 3], [1, 3], [0, 3], [0, 4], [0, 5], [3, 5],
                    [3, 4], [3, 3], [3, 2], [2, 2], [1, 2], [0, 2], [4, 5], [4, 4], [4, 3], [4, 2],
                    [4, 1], [3, 1], [2, 1], [1, 1], [0, 1], [4, 0], [3, 0], [2, 0], [1, 0], [0, 0],
                ]),

            // The degenerate grid: the start cell is the whole answer and no
            // stride is ever walked.
            new SpiralExample(1, 1, 0, 0, [[0, 0]]),

            // One full ring of the smallest square, closed on the last stride.
            new SpiralExample(2, 2, 0, 0, [[0, 0], [0, 1], [1, 1], [1, 0]]),

            // A centered start, where every stride stays in bounds.
            new SpiralExample(
                3, 3, 1, 1,
                [[1, 1], [1, 2], [2, 2], [2, 1], [2, 0], [1, 0], [0, 0], [0, 1], [0, 2]]),

            // A single column, entered from its middle: the right/left strides all
            // fall outside, so only the up/down ones record anything.
            new SpiralExample(4, 1, 2, 0, [[2, 0], [3, 0], [1, 0], [0, 0]]),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SpiralWalkByVisitedSet_LeetCodeExamples_VisitsEveryInBoundsCellInSpiralOrder(
        SpiralExample example)
    {
        var actual = SpiralMatrixIIISolution.SpiralWalkByVisitedSet(
            example.Rows, example.Cols, example.RStart, example.CStart);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SpiralWalkByGrowingStride_LeetCodeExamples_VisitsEveryInBoundsCellInSpiralOrder(
        SpiralExample example)
    {
        var actual = SpiralMatrixIIISolution.SpiralWalkByGrowingStride(
            example.Rows, example.Cols, example.RStart, example.CStart);

        Assert.Equal(example.Expected, actual);
    }

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one LeetCode example,
    // and the grid shape, the start cell and the expected walk are one thing that
    // travels together rather than five arguments a caller must count.
    public readonly record struct SpiralExample(
        int Rows, int Cols, int RStart, int CStart, int[][] Expected);
}
