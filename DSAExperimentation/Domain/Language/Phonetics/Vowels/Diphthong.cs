namespace DSAExperimentation.Domain.Language.Phonetics.Vowels;

// One vowel sound gliding into another within a syllable, as the vowel of "my" glides from a toward
// ɪ. It is one sound of the language, not two vowels in a row, and the IPA writes it as the two
// qualities it moves between: aɪ. The symbol is spelled once, when the value is made.
/// <summary>A vowel gliding from one IPA quality to another within one syllable.</summary>
/// <param name="Start">The quality it starts from.</param>
/// <param name="End">The quality it glides toward.</param>
public readonly record struct Diphthong(Vowel Start, Vowel End)
{
    /// <summary>The two qualities' symbols written together, such as "aɪ".</summary>
    public string Symbol { get; } = Start.Symbol + End.Symbol;
}
