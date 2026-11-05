namespace DSAExperimentation.Domain.Language.Phonetics.Vowels;

// How long a vowel is held. The IPA marks length after the vowel: ː for long (iː), ˑ for half-long,
// and nothing for short. Some languages tell words apart by it alone, as Received Pronunciation's
// FLEECE vowel iː is the long one, so a language's single vowels carry it (Monophthong).
/// <summary>How long a vowel is held, as the IPA marks it.</summary>
public enum VowelLength
{
    /// <summary>Short, unmarked.</summary>
    Short,

    /// <summary>Half-long, marked ˑ.</summary>
    HalfLong,

    /// <summary>Long, marked ː.</summary>
    Long,
}
