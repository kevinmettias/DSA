using DSAExperimentation.Domain.Language.Phonetics.Consonants;
using DSAExperimentation.Domain.Language.Phonetics.Vowels;

namespace DSAExperimentation.Domain.Language.English.Phonology;

// The letter y's two roles. In spelling it is a consonant letter, one of EnglishConsonants and not
// one of the five written vowels, and at the start of a syllable it spells a consonant sound, the
// palatal approximant j of "yes". Elsewhere it spells a vowel sound: the KIT vowel of "gym" and
// "myth", and the PRICE diphthong of "my" and "fly". So "y is sometimes a vowel" is a fact about
// English's sounds, which is why it is stated here and the letter sets keep it a consonant. The
// sounds are Received Pronunciation's, the accent this phonology states.
/// <summary>The letter y: a consonant letter that spells the consonant j and the vowels of KIT and PRICE.</summary>
public static class LetterY
{
    /// <summary>The letter, lowercase.</summary>
    public const char Letter = 'y';

    /// <summary>The consonant sound y spells at the start of a syllable, as in "yes": j.</summary>
    public static Consonant ConsonantSound => IpaConsonants.VoicedPalatalApproximant;

    /// <summary>The single vowel y spells, as in "gym" and "myth": KIT, ɪ.</summary>
    public static Monophthong SingleVowelSound => ReceivedPronunciation.Kit;

    /// <summary>The diphthong y spells, as in "my" and "fly": PRICE, aɪ.</summary>
    public static Diphthong DiphthongSound => ReceivedPronunciation.Price;
}
