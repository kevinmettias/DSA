using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Domain.Games.Chess;

// A chess piece as the moves it makes: the steps it takes, and whether it slides along each square by
// square until the edge or an occupied square stops it, as a rook, a bishop and a queen do, or makes
// one step only, as a king and a knight do. Each piece is a named value carrying its moves, so a
// solution asks ChessPiece.Rook for a rook's lines rather than holding a table it must keep right.
// The steps are grid shapes, GridDirections' own tables, so what chess adds is which piece moves
// along which. Their order is GridDirections' order, and part of the contract: LC 1222 lists the
// attacking queens in the order their lines are walked, so a queen's lines are the Octilinear table's
// order, not a rook's then a bishop's.
/// <summary>A chess piece: its name, the steps it moves along, and whether it slides along them.</summary>
/// <param name="Name">The piece's name, such as "rook".</param>
/// <param name="Moves">The steps the piece moves along, in GridDirections' order.</param>
/// <param name="IsSliding">Whether the piece repeats a step until something stops it, rather than taking it once.</param>
public readonly record struct ChessPiece(string Name, ImmutableArray<GridStep> Moves, bool IsSliding)
{
    /// <summary>The rook: slides along its row and its column.</summary>
    public static ChessPiece Rook { get; } = new("rook", GridDirections.Orthogonal, IsSliding: true);

    /// <summary>The bishop: slides along its two diagonals.</summary>
    public static ChessPiece Bishop { get; } = new("bishop", GridDirections.Diagonal, IsSliding: true);

    /// <summary>The queen: slides along every line a rook or a bishop does, in the Octilinear table's order.</summary>
    public static ChessPiece Queen { get; } = new("queen", GridDirections.Octilinear, IsSliding: true);

    /// <summary>The king: one step to any neighbouring square.</summary>
    public static ChessPiece King { get; } = new("king", GridDirections.Octilinear, IsSliding: false);

    /// <summary>The knight: one jump, two squares one way and one the other.</summary>
    public static ChessPiece Knight { get; } = new("knight", GridDirections.LShaped, IsSliding: false);
}
