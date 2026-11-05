using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Domain.Games.Chess;

// One of a piece's moves, as Betza notation writes one: the steps it may take, how far it may repeat
// a step along a line, and when it may be made. The reach is 1 for a leaper, such as the knight or the
// king, which takes a step once; a number above it for a limited rider, as the pawn's first move rides
// two; and none for a rider, such as the rook, which repeats its step until the board's edge or an
// occupied square stops it. No reach is a missing number rather than a large one, so it cannot be
// mistaken for a count. A rider is blocked by the first occupied square on its line, and may take it
// only if the move's conditions let it capture.
/// <summary>One of a piece's moves: the steps it takes, how far it may repeat one, and when it may be made.</summary>
/// <param name="Steps">The steps the move may take, each the first step of a line it may ride.</param>
/// <param name="Reach">How many times the move may repeat a step along its line: 1 for a leaper, none for a rider with no limit.</param>
/// <param name="When">When the move may be made.</param>
public readonly record struct PieceMove(ImmutableArray<GridStep> Steps, int? Reach, MoveConditions When)
{
    /// <summary>A move that takes one of the steps once, to an empty square or as a capture.</summary>
    /// <param name="steps">The steps the move may take.</param>
    /// <returns>The move.</returns>
    public static PieceMove Leap(ImmutableArray<GridStep> steps) => new(steps, 1, MoveConditions.MovesOrCaptures);

    /// <summary>A move that repeats one of the steps along its line until something stops it, to an empty square or as a capture.</summary>
    /// <param name="steps">The steps the move may ride along.</param>
    /// <returns>The move.</returns>
    public static PieceMove Ride(ImmutableArray<GridStep> steps) => new(steps, null, MoveConditions.MovesOrCaptures);
}
