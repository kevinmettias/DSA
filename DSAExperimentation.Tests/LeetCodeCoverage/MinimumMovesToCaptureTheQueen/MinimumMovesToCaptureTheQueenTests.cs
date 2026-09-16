using DSAExperimentation.LeetCode.MinimumMovesToCaptureTheQueen;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumMovesToCaptureTheQueen;

// Harness only: the algorithms live in MinimumMovesToCaptureTheQueenSolution. One
// test method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke (TwoSumTests precedent).
public sealed class MinimumMovesToCaptureTheQueenTests
{
    public static TheoryData<QueenExample> Examples =>
        new()
        {
            { new QueenExample(Rook: (1, 1), Bishop: (8, 8), Queen: (2, 3), Expected: 2) },
            { new QueenExample(Rook: (5, 3), Bishop: (3, 4), Queen: (5, 2), Expected: 1) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMovesByDestinationEnumeration_LeetCodeExamples_ReturnsFewestMoves(QueenExample example)
    {
        var actual = MinimumMovesToCaptureTheQueenSolution.MinMovesByDestinationEnumeration(
            new ChessSquare(example.Rook.Row, example.Rook.Col),
            new ChessSquare(example.Bishop.Row, example.Bishop.Col),
            new ChessSquare(example.Queen.Row, example.Queen.Col));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMovesByLineOfSight_LeetCodeExamples_ReturnsFewestMoves(QueenExample example)
    {
        var actual = MinimumMovesToCaptureTheQueenSolution.MinMovesByLineOfSight(
            new ChessSquare(example.Rook.Row, example.Rook.Col),
            new ChessSquare(example.Bishop.Row, example.Bishop.Col),
            new ChessSquare(example.Queen.Row, example.Queen.Col));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: where the three pieces stand and the fewest moves that
    // capture the queen. Each square is a (row, column) pair, and the fields name
    // which piece occupies it - a row of six bare ints does not say which pair is the
    // rook's. ChessSquare itself is internal to the solution tier, so the row carries
    // the coordinates and each strategy's call builds the squares.
    public readonly record struct QueenExample(
        (int Row, int Col) Rook,
        (int Row, int Col) Bishop,
        (int Row, int Col) Queen,
        int Expected);
}
