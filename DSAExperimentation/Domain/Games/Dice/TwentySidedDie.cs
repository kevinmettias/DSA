using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Games.Dice;

// The twenty-sided die, the icosahedron of tabletop games, its faces 1 through 20.
/// <summary>A twenty-sided die, faces 1 through 20.</summary>
public readonly struct TwentySidedDie : IDie
{
    /// <inheritdoc/>
    public static ImmutableArray<DieFace> Faces { get; } = DieFace.FairRun(1, 20);
}
