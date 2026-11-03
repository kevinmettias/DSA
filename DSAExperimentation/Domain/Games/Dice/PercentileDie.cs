using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Games.Dice;

// The percentile die, a ten-sided die printed 00 through 90 in tens: rolled with a TenSidedDie for
// the units, the two read 0 through 99.
/// <summary>A percentile die, faces 0 through 90 in tens.</summary>
public readonly struct PercentileDie : IDie
{
    /// <inheritdoc/>
    public static ImmutableArray<DieFace> Faces { get; } = DieFace.FairFaces(0, 10, 20, 30, 40, 50, 60, 70, 80, 90);
}
