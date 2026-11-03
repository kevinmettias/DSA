using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Games.Dice;

// The Fudge die of the FATE game: six faces, two each of -1, 0 and +1, so four of them summed
// range -4 to +4 with zero most likely. A face's value repeats, so the die is its faces, not its values.
/// <summary>A Fudge die: -1, 0 and +1, two faces each.</summary>
public readonly struct FudgeDie : IDie
{
    /// <inheritdoc/>
    public static ImmutableArray<DieFace> Faces { get; } = DieFace.FairFaces(-1, -1, 0, 0, 1, 1);
}
