using DSAExperimentation.LeetCode.RandomFlipMatrix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RandomFlipMatrix;

// Harness only. Both strategies are RandomFlipMatrixSolution's - this file replays the
// two properties LeetCode's Flip/Reset contract guarantees (every cell is visited
// exactly once before the matrix fills, and Reset restores full coverage) against each
// IFlipMatrix implementation, so a failure names the strategy that broke.
public sealed class RandomFlipMatrixTests
{
    public static TheoryData<int, int> Dimensions =>
        new()
        {
            { 2, 3 },
            { 3, 1 },
        };

    [Theory]
    [MemberData(nameof(Dimensions))]
    public void FlipMatrixByListScan_Flip_UntilMatrixFull_VisitsEveryCellExactlyOnce(int rows, int cols) =>
        AssertFlipsUntilFullVisitEveryCellExactlyOnce(
            new RandomFlipMatrixSolution.FlipMatrixByListScan(rows, cols, new Random(1)),
            rows,
            cols);

    [Theory]
    [MemberData(nameof(Dimensions))]
    public void FlipMatrixByHashMapSwapRemove_Flip_UntilMatrixFull_VisitsEveryCellExactlyOnce(int rows, int cols) =>
        AssertFlipsUntilFullVisitEveryCellExactlyOnce(
            new RandomFlipMatrixSolution.FlipMatrixByHashMapSwapRemove(rows, cols, new Random(1)),
            rows,
            cols);

    [Fact]
    public void FlipMatrixByListScan_Reset_AfterPartialFlips_AllowsFullCoverageAgain() =>
        AssertResetAfterPartialFlipsAllowsFullCoverageAgain(
            new RandomFlipMatrixSolution.FlipMatrixByListScan(2, 2, new Random(2)));

    [Fact]
    public void FlipMatrixByHashMapSwapRemove_Reset_AfterPartialFlips_AllowsFullCoverageAgain() =>
        AssertResetAfterPartialFlipsAllowsFullCoverageAgain(
            new RandomFlipMatrixSolution.FlipMatrixByHashMapSwapRemove(2, 2, new Random(2)));

    private static void AssertFlipsUntilFullVisitEveryCellExactlyOnce(
        RandomFlipMatrixSolution.IFlipMatrix matrix, int rows, int cols)
    {
        var seen = new HashSet<(int Row, int Col)>();

        for (var i = 0; i < rows * cols; i++)
        {
            var cell = matrix.Flip();

            Assert.InRange(cell[0], 0, rows - 1);
            Assert.InRange(cell[1], 0, cols - 1);
            Assert.True(seen.Add((cell[0], cell[1])), "Flip returned an already-flipped cell before the matrix was full.");
        }

        Assert.Equal(rows * cols, seen.Count);
    }

    private static void AssertResetAfterPartialFlipsAllowsFullCoverageAgain(
        RandomFlipMatrixSolution.IFlipMatrix matrix)
    {
        matrix.Flip();
        matrix.Flip();

        matrix.Reset();

        var seen = new HashSet<(int Row, int Col)>();
        for (var i = 0; i < 4; i++)
        {
            var cell = matrix.Flip();
            seen.Add((cell[0], cell[1]));
        }

        Assert.Equal(4, seen.Count);
    }
}
