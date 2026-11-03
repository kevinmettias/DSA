using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Games.Dice;

// The smaller of George Sicherman's pair: 1, 2, 2, 3, 3, 4. Rolled with SichermanLargeDie, the sum has
// exactly the distribution of two common six-sided dice, the only other pair of positive dice that does.
/// <summary>The smaller Sicherman die: 1, 2, 2, 3, 3, 4.</summary>
public readonly struct SichermanSmallDie : IDie
{
    /// <inheritdoc/>
    public static ImmutableArray<DieFace> Faces { get; } = DieFace.FairFaces(1, 2, 2, 3, 3, 4);
}
