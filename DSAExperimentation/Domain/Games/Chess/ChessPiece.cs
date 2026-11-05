using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Domain.Games.Chess;

// A chess piece as the moves it makes, each a PieceMove: the steps it takes, its reach and when it may
// be made, which is how Betza notation describes any piece. Most pieces make one move that both moves
// and captures: the rook rides its row and column, the bishop its diagonals, the queen both, the king
// leaps one square any way and the knight one L-shaped jump. The pawn makes three, facing its side's
// way: one step forward to an empty square, a capture one step diagonally forward, and two steps
// forward as its first move, a ride that the square between blocks. En passant and promotion are rules
// of a game in play rather than moves a piece has, and are not stated here. Each piece is a named
// value, so a solution asks ChessPiece.Rook for a rook's lines rather than holding a table it must keep
// right, and the tests hold each preset to its Betza string (BetzaNotation).
//
// The steps are the square lattice's, GridDirections' own tables (Domain/Mathematics/Lattices), so what
// chess adds is which piece moves along which, how far and when. Their order is GridDirections' order,
// and part of the contract: LC 1222 lists the attacking queens in the order their lines are walked, so
// the queen's one move is the Octilinear table, not a rook's move and then a bishop's.
/// <summary>A chess piece: its name and the moves it makes.</summary>
/// <param name="Name">The piece's name, such as "rook".</param>
/// <param name="Moves">The moves the piece makes.</param>
public readonly record struct ChessPiece(string Name, ImmutableArray<PieceMove> Moves)
{
    // How far a pawn's first move may ride forward.
    private const int PawnFirstMoveReach = 2;

    // Both sides' pawns are called pawns; the side they face is the difference.
    private const string PawnName = "pawn";

    // The five pieces that make one move each are built from that move, named, so a search that walks
    // a rook's lines reads RookRide.Steps: one field, where Rook.Moves[0].Steps also checked the list's
    // bounds and loaded its element, which cost LC 999's ray walk about a nanosecond, 18%.

    /// <summary>The rook's one move: a ride along its row and its column.</summary>
    public static PieceMove RookRide { get; } = PieceMove.Ride(GridDirections.Orthogonal);

    /// <summary>The bishop's one move: a ride along its two diagonals.</summary>
    public static PieceMove BishopRide { get; } = PieceMove.Ride(GridDirections.Diagonal);

    /// <summary>The queen's one move: a ride along every line a rook or a bishop does, in the Octilinear table's order.</summary>
    public static PieceMove QueenRide { get; } = PieceMove.Ride(GridDirections.Octilinear);

    /// <summary>The king's one move: a leap of one square to any neighbour.</summary>
    public static PieceMove KingLeap { get; } = PieceMove.Leap(GridDirections.Octilinear);

    /// <summary>The knight's one move: a leap of two squares one way and one the other.</summary>
    public static PieceMove KnightLeap { get; } = PieceMove.Leap(GridDirections.LShaped);

    /// <summary>The rook: rides along its row and its column.</summary>
    public static ChessPiece Rook { get; } = new("rook", [RookRide]);

    /// <summary>The bishop: rides along its two diagonals.</summary>
    public static ChessPiece Bishop { get; } = new("bishop", [BishopRide]);

    /// <summary>The queen: rides along every line a rook or a bishop does, as one move in the Octilinear table's order.</summary>
    public static ChessPiece Queen { get; } = new("queen", [QueenRide]);

    /// <summary>The king: leaps one square to any neighbour.</summary>
    public static ChessPiece King { get; } = new("king", [KingLeap]);

    /// <summary>The knight: leaps two squares one way and one the other.</summary>
    public static ChessPiece Knight { get; } = new("knight", [KnightLeap]);

    /// <summary>White's pawn, which moves up the board.</summary>
    public static ChessPiece WhitePawn { get; } = PawnFacing(ChessSide.White);

    /// <summary>Black's pawn, which moves down the board.</summary>
    public static ChessPiece BlackPawn { get; } = PawnFacing(ChessSide.Black);

    // Betza's mfW cfF ifmW2, facing the side's forward.
    private static ChessPiece PawnFacing(ChessSide side) => new(PawnName,
    [
        new PieceMove([side.Forward], 1, MoveConditions.Moves),
        new PieceMove([side.Forward + GridStep.Left, side.Forward + GridStep.Right], 1, MoveConditions.Captures),
        new PieceMove([side.Forward], PawnFirstMoveReach, MoveConditions.Moves | MoveConditions.FirstMoveOnly),
    ]);
}
