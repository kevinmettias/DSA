using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Language.Scripts.Latin;

// A Latin letter taken apart: the basic letter it is written on and the marks written on it, é as e
// and the acute accent. Most accented letters carry one mark, and some carry two, as Vietnamese ế
// carries a circumflex and an acute; the marks keep the order Unicode's canonical decomposition
// puts them in. A basic letter is its own base with no marks.
/// <summary>A Latin letter as its basic letter and the diacritics written on it.</summary>
/// <param name="BaseLetter">The basic Latin letter, such as 'e' for é, in the letter's case.</param>
/// <param name="Diacritics">The marks, in Unicode's canonical order; empty for a basic letter.</param>
public readonly record struct DecomposedLetter(
    char BaseLetter, ImmutableArray<Diacritic> Diacritics);
