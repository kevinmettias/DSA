using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Language.Phonetics.Vowels;

// The International Phonetic Alphabet's vowel chart, its 28 symbols each a named value carrying the
// cell it names, named as the IPA names them. These are the sounds; a language's vowels are some of
// them, held short or long or gliding one into another, which is the language's configuration
// (IPhonemeInventory). The chart is the IPA's of 2020.
/// <summary>The 28 vowels of the IPA chart, each named for its height, backness and rounding.</summary>
public static class IpaVowels
{
    /// <summary>i, the close front unrounded vowel.</summary>
    public static Vowel CloseFrontUnrounded { get; } =
        new("i", VowelHeight.Close, VowelBackness.Front, Rounding.Unrounded);

    /// <summary>y, the close front rounded vowel.</summary>
    public static Vowel CloseFrontRounded { get; } =
        new("y", VowelHeight.Close, VowelBackness.Front, Rounding.Rounded);

    /// <summary>ɨ, the close central unrounded vowel.</summary>
    public static Vowel CloseCentralUnrounded { get; } =
        new("ɨ", VowelHeight.Close, VowelBackness.Central, Rounding.Unrounded);

    /// <summary>ʉ, the close central rounded vowel.</summary>
    public static Vowel CloseCentralRounded { get; } =
        new("ʉ", VowelHeight.Close, VowelBackness.Central, Rounding.Rounded);

    /// <summary>ɯ, the close back unrounded vowel.</summary>
    public static Vowel CloseBackUnrounded { get; } =
        new("ɯ", VowelHeight.Close, VowelBackness.Back, Rounding.Unrounded);

    /// <summary>u, the close back rounded vowel.</summary>
    public static Vowel CloseBackRounded { get; } =
        new("u", VowelHeight.Close, VowelBackness.Back, Rounding.Rounded);

    /// <summary>ɪ, the near-close near-front unrounded vowel.</summary>
    public static Vowel NearCloseNearFrontUnrounded { get; } =
        new("ɪ", VowelHeight.NearClose, VowelBackness.NearFront, Rounding.Unrounded);

    /// <summary>ʏ, the near-close near-front rounded vowel.</summary>
    public static Vowel NearCloseNearFrontRounded { get; } =
        new("ʏ", VowelHeight.NearClose, VowelBackness.NearFront, Rounding.Rounded);

    /// <summary>ʊ, the near-close near-back rounded vowel.</summary>
    public static Vowel NearCloseNearBackRounded { get; } =
        new("ʊ", VowelHeight.NearClose, VowelBackness.NearBack, Rounding.Rounded);

    /// <summary>e, the close-mid front unrounded vowel.</summary>
    public static Vowel CloseMidFrontUnrounded { get; } =
        new("e", VowelHeight.CloseMid, VowelBackness.Front, Rounding.Unrounded);

    /// <summary>ø, the close-mid front rounded vowel.</summary>
    public static Vowel CloseMidFrontRounded { get; } =
        new("ø", VowelHeight.CloseMid, VowelBackness.Front, Rounding.Rounded);

    /// <summary>ɘ, the close-mid central unrounded vowel.</summary>
    public static Vowel CloseMidCentralUnrounded { get; } =
        new("ɘ", VowelHeight.CloseMid, VowelBackness.Central, Rounding.Unrounded);

    /// <summary>ɵ, the close-mid central rounded vowel.</summary>
    public static Vowel CloseMidCentralRounded { get; } =
        new("ɵ", VowelHeight.CloseMid, VowelBackness.Central, Rounding.Rounded);

    /// <summary>ɤ, the close-mid back unrounded vowel.</summary>
    public static Vowel CloseMidBackUnrounded { get; } =
        new("ɤ", VowelHeight.CloseMid, VowelBackness.Back, Rounding.Unrounded);

    /// <summary>o, the close-mid back rounded vowel.</summary>
    public static Vowel CloseMidBackRounded { get; } =
        new("o", VowelHeight.CloseMid, VowelBackness.Back, Rounding.Rounded);

    /// <summary>ə, the mid central vowel, schwa.</summary>
    public static Vowel MidCentral { get; } =
        new("ə", VowelHeight.Mid, VowelBackness.Central, Rounding.Unspecified);

    /// <summary>ɛ, the open-mid front unrounded vowel.</summary>
    public static Vowel OpenMidFrontUnrounded { get; } =
        new("ɛ", VowelHeight.OpenMid, VowelBackness.Front, Rounding.Unrounded);

    /// <summary>œ, the open-mid front rounded vowel.</summary>
    public static Vowel OpenMidFrontRounded { get; } =
        new("œ", VowelHeight.OpenMid, VowelBackness.Front, Rounding.Rounded);

    /// <summary>ɜ, the open-mid central unrounded vowel.</summary>
    public static Vowel OpenMidCentralUnrounded { get; } =
        new("ɜ", VowelHeight.OpenMid, VowelBackness.Central, Rounding.Unrounded);

    /// <summary>ɞ, the open-mid central rounded vowel.</summary>
    public static Vowel OpenMidCentralRounded { get; } =
        new("ɞ", VowelHeight.OpenMid, VowelBackness.Central, Rounding.Rounded);

    /// <summary>ʌ, the open-mid back unrounded vowel.</summary>
    public static Vowel OpenMidBackUnrounded { get; } =
        new("ʌ", VowelHeight.OpenMid, VowelBackness.Back, Rounding.Unrounded);

    /// <summary>ɔ, the open-mid back rounded vowel.</summary>
    public static Vowel OpenMidBackRounded { get; } =
        new("ɔ", VowelHeight.OpenMid, VowelBackness.Back, Rounding.Rounded);

    /// <summary>æ, the near-open front unrounded vowel.</summary>
    public static Vowel NearOpenFrontUnrounded { get; } =
        new("æ", VowelHeight.NearOpen, VowelBackness.Front, Rounding.Unrounded);

    /// <summary>ɐ, the near-open central vowel.</summary>
    public static Vowel NearOpenCentral { get; } =
        new("ɐ", VowelHeight.NearOpen, VowelBackness.Central, Rounding.Unspecified);

    /// <summary>a, the open front unrounded vowel.</summary>
    public static Vowel OpenFrontUnrounded { get; } =
        new("a", VowelHeight.Open, VowelBackness.Front, Rounding.Unrounded);

    /// <summary>ɶ, the open front rounded vowel.</summary>
    public static Vowel OpenFrontRounded { get; } =
        new("ɶ", VowelHeight.Open, VowelBackness.Front, Rounding.Rounded);

    /// <summary>ɑ, the open back unrounded vowel.</summary>
    public static Vowel OpenBackUnrounded { get; } =
        new("ɑ", VowelHeight.Open, VowelBackness.Back, Rounding.Unrounded);

    /// <summary>ɒ, the open back rounded vowel.</summary>
    public static Vowel OpenBackRounded { get; } =
        new("ɒ", VowelHeight.Open, VowelBackness.Back, Rounding.Rounded);

    /// <summary>The 28 vowels, row by row from close to open and left to right within a row, as the chart lays them out.</summary>
    public static ImmutableArray<Vowel> All { get; } =
    [
        CloseFrontUnrounded, CloseFrontRounded, CloseCentralUnrounded, CloseCentralRounded,
        CloseBackUnrounded, CloseBackRounded,
        NearCloseNearFrontUnrounded, NearCloseNearFrontRounded, NearCloseNearBackRounded,
        CloseMidFrontUnrounded, CloseMidFrontRounded, CloseMidCentralUnrounded,
        CloseMidCentralRounded, CloseMidBackUnrounded, CloseMidBackRounded,
        MidCentral,
        OpenMidFrontUnrounded, OpenMidFrontRounded, OpenMidCentralUnrounded, OpenMidCentralRounded,
        OpenMidBackUnrounded, OpenMidBackRounded,
        NearOpenFrontUnrounded, NearOpenCentral,
        OpenFrontUnrounded, OpenFrontRounded, OpenBackUnrounded, OpenBackRounded,
    ];
}
