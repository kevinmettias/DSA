using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Games.Dice;

// One face of a die: the number it shows, and its weight, how many times as likely it is to come up
// as a face of weight one. A fair die's faces all weigh the same; a loaded one's do not. Faces may
// repeat a value, as a Fudge die's or a Sicherman die's do, so a face is not its value.
/// <summary>A face of a die: the number it shows and how heavily it weighs.</summary>
/// <param name="Value">The number the face shows.</param>
/// <param name="Weight">How likely the face is, relative to the die's other faces; at least zero.</param>
public readonly record struct DieFace(int Value, int Weight)
{
    private const int FairWeight = 1;

    /// <summary>Fair faces numbered in a run: lowest, lowest + 1, and so on, each of weight one.</summary>
    /// <param name="lowest">The lowest face's number.</param>
    /// <param name="count">How many faces.</param>
    /// <returns>The faces, in order.</returns>
    public static ImmutableArray<DieFace> FairRun(int lowest, int count) =>
        [.. Enumerable.Range(lowest, count).Select(Fair)];

    /// <summary>Fair faces showing the given numbers, each of weight one.</summary>
    /// <param name="values">The faces' numbers, in order; a number may repeat.</param>
    /// <returns>The faces, in order.</returns>
    public static ImmutableArray<DieFace> FairFaces(params ReadOnlySpan<int> values)
    {
        var faces = ImmutableArray.CreateBuilder<DieFace>(values.Length);

        foreach (var value in values)
        {
            faces.Add(Fair(value));
        }

        return faces.MoveToImmutable();
    }

    private static DieFace Fair(int value) => new(value, FairWeight);
}
