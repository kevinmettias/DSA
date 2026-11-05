using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Domain.Games.Chess;

// A side of the board and the way it faces. Rows grow downward on the lattice, and White starts on
// the rows nearest the bottom, as a board is drawn from White's seat, so White's forward is Up and
// Black's is Down. A move Betza notation marks forward, such as the pawn's, is forward for its side.
/// <summary>A side of a chess board: its name and the step it calls forward.</summary>
/// <param name="Name">The side's name, such as "white".</param>
/// <param name="Forward">The orthogonal step toward the other side's starting rows.</param>
public readonly record struct ChessSide(string Name, GridStep Forward)
{
    /// <summary>White, which moves up the board.</summary>
    public static ChessSide White { get; } = new("white", GridStep.Up);

    /// <summary>Black, which moves down the board.</summary>
    public static ChessSide Black { get; } = new("black", GridStep.Down);
}
