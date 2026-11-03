using DSAExperimentation.LeetCode.Solutions.MinimumMovesToCaptureTheQueen;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumMovesToCaptureTheQueen;

// Harness only: the algorithms live in MinimumMovesToCaptureTheQueenSolution. One
// test method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke (TwoSumTests precedent).
public sealed partial class MinimumMovesToCaptureTheQueenSolutionTests
{
    public static TheoryData<QueenExample> Examples =>
        new()
        {
            { new QueenExample(Rook: (1, 1), Bishop: (8, 8), Queen: (2, 3), Expected: 2) },
            { new QueenExample(Rook: (5, 3), Bishop: (3, 4), Queen: (5, 2), Expected: 1) },
        };

    // Positions where only one piece's lines reach the queen, which LeetCode's examples do not pin:
    // in its second example the bishop captures whenever the rook does. The rook along its column,
    // the bishop along a diagonal, and the rook's row blocked by the bishop standing in it.
    public static TheoryData<QueenExample> OnePieceCaptures =>
        new()
        {
            { new QueenExample(Rook: (1, 1), Bishop: (2, 3), Queen: (5, 1), Expected: 1) },
            { new QueenExample(Rook: (8, 1), Bishop: (3, 3), Queen: (5, 5), Expected: 1) },
            { new QueenExample(Rook: (1, 1), Bishop: (1, 3), Queen: (1, 5), Expected: 2) },
        };

    [Theory]
    [MemberData(nameof(OnePieceCaptures))]
    public void MinMovesByDestinationEnumeration_OnlyOnePiecesLinesReach_ReturnsFewestMoves(QueenExample example) =>
        MinMovesByDestinationEnumeration_LeetCodeExamples_ReturnsFewestMoves(example);

    [Theory]
    [MemberData(nameof(OnePieceCaptures))]
    public void MinMovesByLineOfSight_OnlyOnePiecesLinesReach_ReturnsFewestMoves(QueenExample example) =>
        MinMovesByLineOfSight_LeetCodeExamples_ReturnsFewestMoves(example);

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
