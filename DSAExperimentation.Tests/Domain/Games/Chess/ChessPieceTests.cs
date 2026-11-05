using DSAExperimentation.Domain.Games.Chess;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Tests.Domain.Games.Chess;

// Each piece's moves: which steps, how far and when. A queen rides exactly a rook's and a bishop's
// lines together, as one move in the Octilinear table's order, which LC 1222 compares its arms in. A
// pawn's three moves face its side's way.
public sealed partial class ChessPieceTests
{
    [Fact]
    public void Rook_RidesAlongItsRowAndColumn()
    {
        var move = Assert.Single(ChessPiece.Rook.Moves);

        Assert.Equal(((int?)null, MoveConditions.MovesOrCaptures), (move.Reach, move.When));
        Assert.Equal<GridStep>(GridDirections.Orthogonal, move.Steps);
    }

    [Fact]
    public void Bishop_RidesAlongItsDiagonals()
    {
        var move = Assert.Single(ChessPiece.Bishop.Moves);

        Assert.Equal(((int?)null, MoveConditions.MovesOrCaptures), (move.Reach, move.When));
        Assert.Equal<GridStep>(GridDirections.Diagonal, move.Steps);
    }

    [Fact]
    public void Queen_RidesAlongTheRooksAndTheBishopsLines() =>
        Assert.Equal(
            ChessPiece.Rook.Moves[0].Steps.Concat(ChessPiece.Bishop.Moves[0].Steps).ToHashSet(),
            Assert.Single(ChessPiece.Queen.Moves).Steps.ToHashSet());

    [Fact]
    public void Queen_RidesInTheOctilinearTablesOrder() =>
        Assert.Equal<GridStep>(GridDirections.Octilinear, Assert.Single(ChessPiece.Queen.Moves).Steps);

    [Fact]
    public void King_LeapsOnceToAnyNeighbour()
    {
        var move = Assert.Single(ChessPiece.King.Moves);

        Assert.Equal(((int?)1, MoveConditions.MovesOrCaptures), (move.Reach, move.When));
        Assert.Equal<GridStep>(GridDirections.Octilinear, move.Steps);
    }

    [Fact]
    public void Knight_LeapsOnce()
    {
        var move = Assert.Single(ChessPiece.Knight.Moves);

        Assert.Equal(((int?)1, MoveConditions.MovesOrCaptures), (move.Reach, move.When));
        Assert.Equal<GridStep>(GridDirections.LShaped, move.Steps);
    }

    // Up one to an empty square, a capture up one diagonally, and up two as its first move; nothing
    // backward or sideways, and no capture straight ahead.
    [Fact]
    public void WhitePawn_MovesUpCapturesDiagonallyUpAndMayMoveTwoFirst() =>
        Assert.Equal(
            [
                (GridStep.Up, (int?)1, MoveConditions.Moves),
                (GridStep.UpLeft, 1, MoveConditions.Captures),
                (GridStep.UpRight, 1, MoveConditions.Captures),
                (GridStep.Up, 2, MoveConditions.Moves | MoveConditions.FirstMoveOnly),
            ],
            Expanded(ChessPiece.WhitePawn));

    [Fact]
    public void BlackPawn_IsWhitesPawnFacingDown() =>
        Assert.Equal(
            Expanded(ChessPiece.WhitePawn).Select(move => (new GridStep(-move.Step.DeltaRow, move.Step.DeltaCol), move.Reach, move.When)),
            Expanded(ChessPiece.BlackPawn));

    // Each piece that makes one move is built from its named move, so reading the move is reading the
    // piece's.
    [Fact]
    public void RookRide_IsTheRooksOneMove() => Assert.Equal(ChessPiece.RookRide, Assert.Single(ChessPiece.Rook.Moves));

    [Fact]
    public void BishopRide_IsTheBishopsOneMove() => Assert.Equal(ChessPiece.BishopRide, Assert.Single(ChessPiece.Bishop.Moves));

    [Fact]
    public void QueenRide_IsTheQueensOneMove() => Assert.Equal(ChessPiece.QueenRide, Assert.Single(ChessPiece.Queen.Moves));

    [Fact]
    public void KingLeap_IsTheKingsOneMove() => Assert.Equal(ChessPiece.KingLeap, Assert.Single(ChessPiece.King.Moves));

    [Fact]
    public void KnightLeap_IsTheKnightsOneMove() => Assert.Equal(ChessPiece.KnightLeap, Assert.Single(ChessPiece.Knight.Moves));

    private static List<(GridStep Step, int? Reach, MoveConditions When)> Expanded(ChessPiece piece) =>
        [.. piece.Moves.SelectMany(move => move.Steps.Select(step => (step, move.Reach, move.When)))];
}
