using DSAExperimentation.Domain.Games.Chess;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Tests.Domain.Games.Chess;

// Each piece's moves, and whether it slides. A queen moves along exactly a rook's and a bishop's lines
// together, in the Octilinear table's order, which LC 1222 compares its arms in.
public sealed partial class ChessPieceTests
{
    [Fact]
    public void Rook_SlidesAlongItsRowAndColumn()
    {
        Assert.True(ChessPiece.Rook.IsSliding);
        Assert.All(ChessPiece.Rook.Moves, step => Assert.True((step.DeltaRow == 0) != (step.DeltaCol == 0)));
    }

    [Fact]
    public void Bishop_SlidesAlongItsDiagonals()
    {
        Assert.True(ChessPiece.Bishop.IsSliding);
        Assert.All(ChessPiece.Bishop.Moves, step => Assert.True(Math.Abs(step.DeltaRow) == 1 && Math.Abs(step.DeltaCol) == 1));
    }

    [Fact]
    public void Queen_SlidesAlongTheRooksAndTheBishopsLines() =>
        Assert.Equal(
            ChessPiece.Rook.Moves.Concat(ChessPiece.Bishop.Moves).ToHashSet(),
            ChessPiece.Queen.Moves.ToHashSet());

    [Fact]
    public void Queen_MovesInTheKingTablesOrder() => Assert.Equal<GridStep>(GridDirections.Octilinear, ChessPiece.Queen.Moves);

    [Fact]
    public void King_StepsOnceToAnyNeighbour()
    {
        Assert.False(ChessPiece.King.IsSliding);
        Assert.Equal<GridStep>(GridDirections.Octilinear, ChessPiece.King.Moves);
    }

    [Fact]
    public void Knight_JumpsOnce()
    {
        Assert.False(ChessPiece.Knight.IsSliding);
        Assert.Equal<GridStep>(GridDirections.LShaped, ChessPiece.Knight.Moves);
    }
}
