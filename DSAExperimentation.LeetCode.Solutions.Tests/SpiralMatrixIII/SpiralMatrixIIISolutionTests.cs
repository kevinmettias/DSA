using DSAExperimentation.LeetCode.Solutions.SpiralMatrixIII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SpiralMatrixIII;

// Harness only: both strategies live in SpiralMatrixIIISolution and are asserted
// against the same cases, so a failure names the strategy that broke. The
// visited-set arm was previously only a benchmark baseline and nothing asserted
// it; it is under test here for the first time.
public sealed partial class SpiralMatrixIIISolutionTests
{
    public static TheoryData<SpiralCase> Cases =>
        new()
        {
            // The degenerate grid: the start cell is the whole answer and no
            // stride is ever walked.
            new SpiralCase(1, 1, 0, 0, [[0, 0]]),

            // One full ring of the smallest square, closed on the last stride.
            new SpiralCase(2, 2, 0, 0, [[0, 0], [0, 1], [1, 1], [1, 0]]),

            // A single column, entered from its middle: the right/left strides all
            // fall outside, so only the up/down ones record anything.
            new SpiralCase(4, 1, 2, 0, [[2, 0], [3, 0], [1, 0], [0, 0]]),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SpiralWalkByVisitedSet_Cases_VisitsEveryInBoundsCellInSpiralOrder(
        SpiralCase testCase)
    {
        var actual = SpiralMatrixIIISolution.SpiralWalkByVisitedSet(
            testCase.Rows, testCase.Cols, testCase.RStart, testCase.CStart);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SpiralWalkByGrowingStride_Cases_VisitsEveryInBoundsCellInSpiralOrder(
        SpiralCase testCase)
    {
        var actual = SpiralMatrixIIISolution.SpiralWalkByGrowingStride(
            testCase.Rows, testCase.Cols, testCase.RStart, testCase.CStart);

        Assert.Equal(testCase.Expected, actual);
    }

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one case, and the
    // grid shape, the start cell and the expected walk are one thing that travels
    // together rather than five arguments a caller must count.
    public readonly record struct SpiralCase(
        int Rows, int Cols, int RStart, int CStart, int[][] Expected);
}
