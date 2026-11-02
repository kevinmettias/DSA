using System.Collections.Immutable;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Domain.Chess;

// Chess as LC 999, LC 1222 and LC 2056 state it: an 8x8 board, a rook moving along its row and its
// column, a bishop along its two diagonals, and a queen along all of them, each sliding square by
// square until the edge or an occupied square stops it. These are the problems' *what*
// (ARCHITECTURE §17.6). The lines themselves are grid shapes, GridDirections' own, so what chess
// adds is which piece moves along which. How a solution walks a line and what it checks at each
// square stay with each strategy; the baselines keep their own copies (§17.5). A knight's and a
// king's moves stay GridDirections' Knight and King: the knight problems here are played on
// boards other than chess's, and no problem here moves a king.
//
// Each set's order is GridDirections' order, and part of the contract: LC 1222 lists the attacking
// queens in the order their lines are walked, and its arms are compared in that order, so a queen's
// lines are the King table's order, not a rook's then a bishop's.
/// <summary>The chess board's size, and the lines a rook, a bishop and a queen move along.</summary>
public static class ChessRules
{
    /// <summary>The number of rows, and of columns, on the board.</summary>
    public const int BoardSize = 8;

    /// <summary>The four directions a rook moves in, along its row and its column.</summary>
    public static ImmutableArray<(int DeltaRow, int DeltaCol)> RookLines => GridDirections.Orthogonal;

    /// <summary>The four directions a bishop moves in, along its two diagonals.</summary>
    public static ImmutableArray<(int DeltaRow, int DeltaCol)> BishopLines => GridDirections.Diagonal;

    /// <summary>The eight directions a queen moves in, a rook's and a bishop's together.</summary>
    public static ImmutableArray<(int DeltaRow, int DeltaCol)> QueenLines => GridDirections.King;
}
