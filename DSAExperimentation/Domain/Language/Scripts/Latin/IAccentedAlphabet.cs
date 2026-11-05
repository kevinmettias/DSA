using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Language.Scripts.Latin;

// A language's Latin alphabet beyond the 26 basic letters: the accented letters it writes, each a
// basic letter with diacritics, and the letters of its own that are no basic letter with a mark, as
// German's ß and French's æ and œ. Accented letters are another alphabet's, not English's (the
// owner's ruling, 2026-11-05), so each language is a preset of its own. Whether the language counts
// an accented letter as a letter of its alphabet, as Spanish counts ñ, or as its base letter with a
// mark, as it counts á, is its own collation and is not stated here.
/// <summary>A Latin alphabet's letters beyond the 26 basic ones.</summary>
public interface IAccentedAlphabet
{
    /// <summary>The accented letters, lowercase, each a basic letter with diacritics.</summary>
    static abstract ImmutableArray<char> AccentedLetters { get; }

    /// <summary>The letters, lowercase, that are neither basic letters nor basic letters with diacritics.</summary>
    static abstract ImmutableArray<char> OtherLetters { get; }
}
