using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Language.Scripts.Latin;

// German's letters beyond the basic 26: the umlauts ä, ö and ü, each a vowel with the diaeresis
// mark, and ß, the sharp s, a letter Unicode does not take apart and which capitalizes to SS, so it
// is German's other letter and no accented one.
/// <summary>The German alphabet's umlauts ä, ö and ü, and ß.</summary>
public readonly struct GermanAlphabet : IAccentedAlphabet
{
    /// <inheritdoc/>
    public static ImmutableArray<char> AccentedLetters { get; } = ['ä', 'ö', 'ü'];

    /// <inheritdoc/>
    public static ImmutableArray<char> OtherLetters { get; } = ['ß'];
}
