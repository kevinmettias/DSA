using System.Collections.Immutable;
using DSAExperimentation.Domain.Language.Phonetics;
using DSAExperimentation.Domain.Language.Phonetics.Consonants;
using DSAExperimentation.Domain.Language.Phonetics.Vowels;

namespace DSAExperimentation.Domain.Language.English.Phonology;

// The phonemes of Received Pronunciation, the accent of English that British dictionaries
// transcribe, in their traditional transcription (Gimson's): 12 single vowels, 8 diphthongs and 24
// consonants, the 44 sounds of English as they are usually counted. Each is a sound of the IPA's
// charts, so the accent is a configuration of the IPA, not a list of its own. Each vowel is named
// by its lexical set (Wells, 1982), the keyword of the words that share it, which names it in every
// accent: KIT is the vowel of "kit", ɪ here. Accents differ in their vowels, so another accent,
// General American with its r-coloured vowels, would be another configuration. Two transcription
// conventions are the IPA's rather than the dictionaries': r is ɹ, the approximant English says,
// since the IPA's r is a trill; and the affricates are written with the tie bar, t͡ʃ and d͡ʒ, so a
// transcription can tell "why choose" from "white shoes", in which t and ʃ only meet.
/// <summary>Received Pronunciation's 44 phonemes: 12 single vowels, 8 diphthongs and 24 consonants.</summary>
public readonly struct ReceivedPronunciation : IPhonemeInventory
{
    /// <summary>FLEECE, iː.</summary>
    public static Monophthong Fleece { get; } =
        new(IpaVowels.CloseFrontUnrounded, VowelLength.Long);

    /// <summary>KIT, ɪ.</summary>
    public static Monophthong Kit { get; } =
        new(IpaVowels.NearCloseNearFrontUnrounded, VowelLength.Short);

    /// <summary>DRESS, e.</summary>
    public static Monophthong Dress { get; } =
        new(IpaVowels.CloseMidFrontUnrounded, VowelLength.Short);

    /// <summary>TRAP, æ.</summary>
    public static Monophthong Trap { get; } =
        new(IpaVowels.NearOpenFrontUnrounded, VowelLength.Short);

    /// <summary>STRUT, ʌ.</summary>
    public static Monophthong Strut { get; } =
        new(IpaVowels.OpenMidBackUnrounded, VowelLength.Short);

    /// <summary>PALM, ɑː, also the vowel of BATH and START.</summary>
    public static Monophthong Palm { get; } = new(IpaVowels.OpenBackUnrounded, VowelLength.Long);

    /// <summary>LOT, ɒ.</summary>
    public static Monophthong Lot { get; } = new(IpaVowels.OpenBackRounded, VowelLength.Short);

    /// <summary>THOUGHT, ɔː, also the vowel of NORTH and FORCE.</summary>
    public static Monophthong Thought { get; } =
        new(IpaVowels.OpenMidBackRounded, VowelLength.Long);

    /// <summary>FOOT, ʊ.</summary>
    public static Monophthong Foot { get; } =
        new(IpaVowels.NearCloseNearBackRounded, VowelLength.Short);

    /// <summary>GOOSE, uː.</summary>
    public static Monophthong Goose { get; } = new(IpaVowels.CloseBackRounded, VowelLength.Long);

    /// <summary>NURSE, ɜː.</summary>
    public static Monophthong Nurse { get; } =
        new(IpaVowels.OpenMidCentralUnrounded, VowelLength.Long);

    /// <summary>COMMA, ə, the unstressed schwa.</summary>
    public static Monophthong Comma { get; } = new(IpaVowels.MidCentral, VowelLength.Short);

    /// <summary>FACE, eɪ.</summary>
    public static Diphthong Face { get; } =
        new(IpaVowels.CloseMidFrontUnrounded, IpaVowels.NearCloseNearFrontUnrounded);

    /// <summary>PRICE, aɪ.</summary>
    public static Diphthong Price { get; } =
        new(IpaVowels.OpenFrontUnrounded, IpaVowels.NearCloseNearFrontUnrounded);

    /// <summary>CHOICE, ɔɪ.</summary>
    public static Diphthong Choice { get; } =
        new(IpaVowels.OpenMidBackRounded, IpaVowels.NearCloseNearFrontUnrounded);

    /// <summary>GOAT, əʊ.</summary>
    public static Diphthong Goat { get; } =
        new(IpaVowels.MidCentral, IpaVowels.NearCloseNearBackRounded);

    /// <summary>MOUTH, aʊ.</summary>
    public static Diphthong Mouth { get; } =
        new(IpaVowels.OpenFrontUnrounded, IpaVowels.NearCloseNearBackRounded);

    /// <summary>NEAR, ɪə.</summary>
    public static Diphthong Near { get; } =
        new(IpaVowels.NearCloseNearFrontUnrounded, IpaVowels.MidCentral);

    /// <summary>SQUARE, eə.</summary>
    public static Diphthong Square { get; } =
        new(IpaVowels.CloseMidFrontUnrounded, IpaVowels.MidCentral);

    /// <summary>CURE, ʊə.</summary>
    public static Diphthong Cure { get; } =
        new(IpaVowels.NearCloseNearBackRounded, IpaVowels.MidCentral);

    /// <inheritdoc/>
    public static ImmutableArray<Monophthong> Monophthongs { get; } =
        [Fleece, Kit, Dress, Trap, Strut, Palm, Lot, Thought, Foot, Goose, Nurse, Comma];

    /// <inheritdoc/>
    public static ImmutableArray<Diphthong> Diphthongs { get; } =
        [Face, Price, Choice, Goat, Mouth, Near, Square, Cure];

    /// <inheritdoc/>
    public static ImmutableArray<Consonant> Consonants { get; } =
    [
        IpaConsonants.VoicelessBilabialPlosive, IpaConsonants.VoicedBilabialPlosive,
        IpaConsonants.VoicelessAlveolarPlosive, IpaConsonants.VoicedAlveolarPlosive,
        IpaConsonants.VoicelessVelarPlosive, IpaConsonants.VoicedVelarPlosive,
        IpaConsonants.VoicelessPostalveolarAffricate, IpaConsonants.VoicedPostalveolarAffricate,
        IpaConsonants.VoicelessLabiodentalFricative, IpaConsonants.VoicedLabiodentalFricative,
        IpaConsonants.VoicelessDentalFricative, IpaConsonants.VoicedDentalFricative,
        IpaConsonants.VoicelessAlveolarFricative, IpaConsonants.VoicedAlveolarFricative,
        IpaConsonants.VoicelessPostalveolarFricative, IpaConsonants.VoicedPostalveolarFricative,
        IpaConsonants.VoicelessGlottalFricative,
        IpaConsonants.VoicedBilabialNasal, IpaConsonants.VoicedAlveolarNasal,
        IpaConsonants.VoicedVelarNasal,
        IpaConsonants.VoicedAlveolarLateralApproximant, IpaConsonants.VoicedAlveolarApproximant,
        IpaConsonants.VoicedPalatalApproximant, IpaConsonants.VoicedLabialVelarApproximant,
    ];
}
