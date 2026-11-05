using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Language.Scripts.Latin;

// Spanish's letters beyond the basic 26: the five vowels with an acute accent, which marks a
// stressed syllable; ü, whose diaeresis says the u is spoken after g, as in pingüino; and ñ, n with
// a tilde, which Spanish counts as a letter of its own. Every one is a basic letter with a mark.
/// <summary>The Spanish alphabet's accented letters: á, é, í, ó, ú, ü and ñ.</summary>
public readonly struct SpanishAlphabet : IAccentedAlphabet
{
    /// <inheritdoc/>
    public static ImmutableArray<char> AccentedLetters { get; } =
        ['á', 'é', 'í', 'ó', 'ú', 'ü', 'ñ'];

    /// <inheritdoc/>
    public static ImmutableArray<char> OtherLetters { get; } = [];
}
