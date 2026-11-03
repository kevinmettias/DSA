using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Games.Dice;

// The ten-sided die, numbered 0 through 9 as most are printed, not 1 through 10.
/// <summary>A ten-sided die, faces 0 through 9.</summary>
public readonly struct TenSidedDie : IDie
{
    /// <inheritdoc/>
    public static ImmutableArray<DieFace> Faces { get; } = DieFace.FairRun(0, 10);
}
