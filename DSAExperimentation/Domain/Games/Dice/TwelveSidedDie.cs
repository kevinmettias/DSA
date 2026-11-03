using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Games.Dice;

// The twelve-sided die, the dodecahedron of tabletop games, its faces 1 through 12.
/// <summary>A twelve-sided die, faces 1 through 12.</summary>
public readonly struct TwelveSidedDie : IDie
{
    /// <inheritdoc/>
    public static ImmutableArray<DieFace> Faces { get; } = DieFace.FairRun(1, 12);
}
