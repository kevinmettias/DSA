using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Games.Dice;

// The eight-sided die, the octahedron of tabletop games, its faces 1 through 8.
/// <summary>An eight-sided die, faces 1 through 8.</summary>
public readonly struct EightSidedDie : IDie
{
    /// <inheritdoc/>
    public static ImmutableArray<DieFace> Faces { get; } = DieFace.FairRun(1, 8);
}
