using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Games.Dice;

// The larger of George Sicherman's pair: 1, 3, 4, 5, 6, 8. Rolled with SichermanSmallDie, the sum has
// exactly the distribution of two common six-sided dice.
/// <summary>The larger Sicherman die: 1, 3, 4, 5, 6, 8.</summary>
public readonly struct SichermanLargeDie : IDie
{
    /// <inheritdoc/>
    public static ImmutableArray<DieFace> Faces { get; } = DieFace.FairFaces(1, 3, 4, 5, 6, 8);
}
