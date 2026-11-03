using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Games.Dice;

// What follows from a die's faces, read from its IDie witness once: how many faces it has, its lowest
// and highest numbers, the total of its weights, and whether it is fair. Each is a primitive static
// readonly field of an instantiation over a struct, so the JIT reads it as a constant in a caller's
// loop. A face's probability is its weight over the total, exactly; drawing a roll is a sampler's.
/// <summary>The faces of a die, and what follows from them.</summary>
/// <typeparam name="TDie">The die, such as <see cref="SixSidedDie"/>.</typeparam>
public static class Die<TDie>
    where TDie : struct, IDie
{
    /// <summary>The die's faces, in order.</summary>
    public static ImmutableArray<DieFace> Faces => TDie.Faces;

    /// <summary>How many faces the die has.</summary>
    public static int FaceCount { get; } = TDie.Faces.Length;

    /// <summary>The lowest number on any face.</summary>
    public static int LowestFace { get; } = TDie.Faces.Min(face => face.Value);

    /// <summary>The highest number on any face.</summary>
    public static int HighestFace { get; } = TDie.Faces.Max(face => face.Value);

    /// <summary>The total of the faces' weights: a face's probability is its weight over this.</summary>
    public static int TotalWeight { get; } = TDie.Faces.Sum(face => face.Weight);

    /// <summary>Whether every face weighs the same.</summary>
    public static bool IsFair { get; } = TDie.Faces.All(face => face.Weight == TDie.Faces[0].Weight);

    /// <summary>The faces' weights, in face order, for a sampler to draw from.</summary>
    public static ImmutableArray<int> Weights { get; } = [.. TDie.Faces.Select(face => face.Weight)];
}
