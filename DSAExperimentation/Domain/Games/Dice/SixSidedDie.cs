using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Games.Dice;

// The common cube die, its faces showing 1 through 6, as LC 2318 rolls it.
/// <summary>A six-sided die, faces 1 through 6.</summary>
public readonly struct SixSidedDie : IDie
{
    /// <inheritdoc/>
    public static ImmutableArray<DieFace> Faces { get; } = DieFace.FairRun(1, 6);
}
