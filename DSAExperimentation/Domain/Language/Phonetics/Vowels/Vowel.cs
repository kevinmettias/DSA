namespace DSAExperimentation.Domain.Language.Phonetics.Vowels;

// A vowel as the IPA writes it: its symbol and the cell of the vowel chart it names, given in the
// order the IPA's own names read, so i is the close front unrounded vowel. The cell is the sound;
// the symbol is how the alphabet writes it. A vowel here is a quality alone: how long a language
// holds it is the language's (Monophthong), as is one vowel gliding into another (Diphthong).
/// <summary>A vowel of the IPA chart: its symbol, height, backness and rounding.</summary>
/// <param name="Symbol">The IPA symbol, such as "i".</param>
/// <param name="Height">The chart's row.</param>
/// <param name="Backness">The chart's column.</param>
/// <param name="Rounding">Whether the lips round.</param>
public readonly record struct Vowel(
    string Symbol, VowelHeight Height, VowelBackness Backness, Rounding Rounding);
