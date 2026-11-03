using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Games.Dice;

// The four-sided die, the tetrahedron of tabletop games, its faces 1 through 4.
/// <summary>A four-sided die, faces 1 through 4.</summary>
public readonly struct FourSidedDie : IDie
{
    /// <inheritdoc/>
    public static ImmutableArray<DieFace> Faces { get; } = DieFace.FairRun(1, 4);
}
