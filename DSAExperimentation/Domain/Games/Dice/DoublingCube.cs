using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Games.Dice;

// Backgammon's doubling cube: its faces are the stakes, 2 through 64 in powers of two. It is turned to
// a face rather than rolled, but its faces are a die's.
/// <summary>Backgammon's doubling cube, faces 2 through 64 in powers of two.</summary>
public readonly struct DoublingCube : IDie
{
    /// <inheritdoc/>
    public static ImmutableArray<DieFace> Faces { get; } = DieFace.FairFaces(2, 4, 8, 16, 32, 64);
}
