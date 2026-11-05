namespace DSAExperimentation.Domain.Language.Phonetics.Vowels;

// A single vowel of a language: one quality from the IPA chart, held for the length the language
// gives it, and written as the quality's symbol followed by the IPA's length mark, so the long
// close front unrounded vowel is iː. The symbol is spelled once, when the value is made.
/// <summary>A language's single vowel: an IPA vowel and how long it is held.</summary>
/// <param name="Quality">The vowel of the IPA chart.</param>
/// <param name="Length">How long it is held.</param>
public readonly record struct Monophthong(Vowel Quality, VowelLength Length)
{
    private const string HalfLongMark = "ˑ";
    private const string LongMark = "ː";

    /// <summary>The vowel's symbol with its length mark, such as "iː"; a short vowel's is the quality's alone.</summary>
    public string Symbol { get; } = Quality.Symbol + MarkOf(Length);

    private static string MarkOf(VowelLength length) => length switch
    {
        VowelLength.HalfLong => HalfLongMark,
        VowelLength.Long => LongMark,
        _ => string.Empty,
    };
}
