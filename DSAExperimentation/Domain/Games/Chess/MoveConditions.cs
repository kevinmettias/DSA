namespace DSAExperimentation.Domain.Games.Chess;

// When a move may be made, as Betza notation's modifiers say it: m, a move to an empty square only;
// c, a capture only; and i, only as the piece's first move. A move with neither Moves nor Captures
// could never be made, so every move carries at least one of them.
/// <summary>When a piece may make one of its moves.</summary>
[Flags]
public enum MoveConditions
{
    /// <summary>The move may go to an empty square.</summary>
    Moves = 1,

    /// <summary>The move may capture a piece of the other side.</summary>
    Captures = 2,

    /// <summary>The move may go to an empty square or capture, as most pieces' moves may.</summary>
    MovesOrCaptures = Moves | Captures,

    /// <summary>The move may be made only as the piece's first move.</summary>
    FirstMoveOnly = 4,
}
