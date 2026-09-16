using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for SurroundedRegionsWorkloads (ARCHITECTURE 17.7): LC 130's board is a square of
// 'X'/'O' cells, and both strategies need real work on both sides of the problem - border-reaching
// regions to preserve and interior regions to flip - rather than a board that is a single letter.
public sealed partial class SurroundedRegionsWorkloadsTests
{
    private const int Size = 32;
    private const int Seed = 130; // LC problem number
    private const char Open = 'O';
    private const char Blocked = 'X';
    private const int FewestCellsOfEitherKind = 1;

    [Fact]
    public void BuildBoard_Size_ReturnsASquareBoardOfThatShape()
    {
        var board = SurroundedRegionsWorkloads.BuildBoard(Size, Seed);

        Assert.Equal(Size, board.Length);
        Assert.All(board, row => Assert.Equal(Size, row.Length));
    }

    [Fact]
    public void BuildBoard_EveryCell_IsEitherOpenOrBlocked() =>
        Assert.All(
            SurroundedRegionsWorkloads.BuildBoard(Size, Seed).SelectMany(row => row),
            cell => Assert.True(cell == Open || cell == Blocked));

    // Which cell is open is a coin flip per cell, so the structural fact is what is asserted: the
    // board leaves both letters present, which is the implication both strategies need - 'O' regions
    // to flood from the border and 'X' cells to keep between them.
    [Fact]
    public void BuildBoard_Board_LeavesBothLettersPresentToFloodFill()
    {
        var cells = SurroundedRegionsWorkloads.BuildBoard(Size, Seed).SelectMany(row => row).ToList();

        Assert.InRange(
            cells.Count(cell => cell == Open),
            FewestCellsOfEitherKind,
            cells.Count - FewestCellsOfEitherKind);
    }

    [Fact]
    public void BuildBoard_SameSeed_ReturnsTheSameBoard() =>
        Assert.Equal(
            AnswerText.Of(SurroundedRegionsWorkloads.BuildBoard(Size, Seed)),
            AnswerText.Of(SurroundedRegionsWorkloads.BuildBoard(Size, Seed)));
}
