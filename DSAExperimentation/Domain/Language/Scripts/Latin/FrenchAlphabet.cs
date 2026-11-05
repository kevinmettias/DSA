using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Language.Scripts.Latin;

// French's letters beyond the basic 26: vowels with the grave, acute and circumflex accents, ç with
// its cedilla, and the vowels with a diaeresis, which says a vowel is spoken apart from the one
// before it, as in naïf and Noël. Its ligatures æ and œ, as in œuvre, are letters Unicode does not
// take apart, so they are its other letters.
/// <summary>The French alphabet's accented letters, and its ligatures æ and œ.</summary>
public readonly struct FrenchAlphabet : IAccentedAlphabet
{
    /// <inheritdoc/>
    public static ImmutableArray<char> AccentedLetters { get; } =
        ['à', 'â', 'ç', 'é', 'è', 'ê', 'ë', 'î', 'ï', 'ô', 'ù', 'û', 'ü', 'ÿ'];

    /// <inheritdoc/>
    public static ImmutableArray<char> OtherLetters { get; } = ['æ', 'œ'];
}
