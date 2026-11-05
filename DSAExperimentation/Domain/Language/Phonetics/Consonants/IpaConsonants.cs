using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Language.Phonetics.Consonants;

// The International Phonetic Alphabet's consonants, each a named value carrying how it is made,
// named as the IPA names them. The pulmonic chart, consonants made with air pushed out from the
// lungs, holds 59 symbols in rows by manner and columns by place, voiceless left of voiced; the
// chart's other symbols add sounds made at two places at once, such as w, or between its columns.
// The four common affricates are built from their stop and fricative. The chart is the IPA's of
// 2020. Two of its parts are not stated here. ɧ, which the chart describes only as ʃ and x at once,
// has no one place to give it. The non-pulmonic consonants, clicks, implosives and ejectives, use
// another airstream, which is a feature of its own and would need its own axis. The voiced velar
// plosive is ɡ, the IPA's own letter, U+0261, not the g of a keyboard, which the IPA accepts in its
// place.
/// <summary>The IPA's consonants: the pulmonic chart, its other symbols, and the common affricates.</summary>
public static class IpaConsonants
{
    /// <summary>p, the voiceless bilabial plosive.</summary>
    public static Consonant VoicelessBilabialPlosive { get; } =
        new("p", Voicing.Voiceless, Place.Bilabial, Manner.Plosive);

    /// <summary>b, the voiced bilabial plosive.</summary>
    public static Consonant VoicedBilabialPlosive { get; } =
        new("b", Voicing.Voiced, Place.Bilabial, Manner.Plosive);

    /// <summary>t, the voiceless alveolar plosive.</summary>
    public static Consonant VoicelessAlveolarPlosive { get; } =
        new("t", Voicing.Voiceless, Place.Alveolar, Manner.Plosive);

    /// <summary>d, the voiced alveolar plosive.</summary>
    public static Consonant VoicedAlveolarPlosive { get; } =
        new("d", Voicing.Voiced, Place.Alveolar, Manner.Plosive);

    /// <summary>ʈ, the voiceless retroflex plosive.</summary>
    public static Consonant VoicelessRetroflexPlosive { get; } =
        new("ʈ", Voicing.Voiceless, Place.Retroflex, Manner.Plosive);

    /// <summary>ɖ, the voiced retroflex plosive.</summary>
    public static Consonant VoicedRetroflexPlosive { get; } =
        new("ɖ", Voicing.Voiced, Place.Retroflex, Manner.Plosive);

    /// <summary>c, the voiceless palatal plosive.</summary>
    public static Consonant VoicelessPalatalPlosive { get; } =
        new("c", Voicing.Voiceless, Place.Palatal, Manner.Plosive);

    /// <summary>ɟ, the voiced palatal plosive.</summary>
    public static Consonant VoicedPalatalPlosive { get; } =
        new("ɟ", Voicing.Voiced, Place.Palatal, Manner.Plosive);

    /// <summary>k, the voiceless velar plosive.</summary>
    public static Consonant VoicelessVelarPlosive { get; } =
        new("k", Voicing.Voiceless, Place.Velar, Manner.Plosive);

    /// <summary>ɡ, the voiced velar plosive, written with the IPA's own letter.</summary>
    public static Consonant VoicedVelarPlosive { get; } =
        new("ɡ", Voicing.Voiced, Place.Velar, Manner.Plosive);

    /// <summary>q, the voiceless uvular plosive.</summary>
    public static Consonant VoicelessUvularPlosive { get; } =
        new("q", Voicing.Voiceless, Place.Uvular, Manner.Plosive);

    /// <summary>ɢ, the voiced uvular plosive.</summary>
    public static Consonant VoicedUvularPlosive { get; } =
        new("ɢ", Voicing.Voiced, Place.Uvular, Manner.Plosive);

    /// <summary>ʔ, the glottal stop, voiceless since the closed glottis cannot vibrate.</summary>
    public static Consonant VoicelessGlottalPlosive { get; } =
        new("ʔ", Voicing.Voiceless, Place.Glottal, Manner.Plosive);

    /// <summary>m, the voiced bilabial nasal.</summary>
    public static Consonant VoicedBilabialNasal { get; } =
        new("m", Voicing.Voiced, Place.Bilabial, Manner.Nasal);

    /// <summary>ɱ, the voiced labiodental nasal.</summary>
    public static Consonant VoicedLabiodentalNasal { get; } =
        new("ɱ", Voicing.Voiced, Place.Labiodental, Manner.Nasal);

    /// <summary>n, the voiced alveolar nasal.</summary>
    public static Consonant VoicedAlveolarNasal { get; } =
        new("n", Voicing.Voiced, Place.Alveolar, Manner.Nasal);

    /// <summary>ɳ, the voiced retroflex nasal.</summary>
    public static Consonant VoicedRetroflexNasal { get; } =
        new("ɳ", Voicing.Voiced, Place.Retroflex, Manner.Nasal);

    /// <summary>ɲ, the voiced palatal nasal.</summary>
    public static Consonant VoicedPalatalNasal { get; } =
        new("ɲ", Voicing.Voiced, Place.Palatal, Manner.Nasal);

    /// <summary>ŋ, the voiced velar nasal.</summary>
    public static Consonant VoicedVelarNasal { get; } =
        new("ŋ", Voicing.Voiced, Place.Velar, Manner.Nasal);

    /// <summary>ɴ, the voiced uvular nasal.</summary>
    public static Consonant VoicedUvularNasal { get; } =
        new("ɴ", Voicing.Voiced, Place.Uvular, Manner.Nasal);

    /// <summary>ʙ, the voiced bilabial trill.</summary>
    public static Consonant VoicedBilabialTrill { get; } =
        new("ʙ", Voicing.Voiced, Place.Bilabial, Manner.Trill);

    /// <summary>r, the voiced alveolar trill.</summary>
    public static Consonant VoicedAlveolarTrill { get; } =
        new("r", Voicing.Voiced, Place.Alveolar, Manner.Trill);

    /// <summary>ʀ, the voiced uvular trill.</summary>
    public static Consonant VoicedUvularTrill { get; } =
        new("ʀ", Voicing.Voiced, Place.Uvular, Manner.Trill);

    /// <summary>ⱱ, the voiced labiodental flap.</summary>
    public static Consonant VoicedLabiodentalFlap { get; } =
        new("ⱱ", Voicing.Voiced, Place.Labiodental, Manner.TapOrFlap);

    /// <summary>ɾ, the voiced alveolar tap.</summary>
    public static Consonant VoicedAlveolarTap { get; } =
        new("ɾ", Voicing.Voiced, Place.Alveolar, Manner.TapOrFlap);

    /// <summary>ɽ, the voiced retroflex flap.</summary>
    public static Consonant VoicedRetroflexFlap { get; } =
        new("ɽ", Voicing.Voiced, Place.Retroflex, Manner.TapOrFlap);

    /// <summary>ɸ, the voiceless bilabial fricative.</summary>
    public static Consonant VoicelessBilabialFricative { get; } =
        new("ɸ", Voicing.Voiceless, Place.Bilabial, Manner.Fricative);

    /// <summary>β, the voiced bilabial fricative.</summary>
    public static Consonant VoicedBilabialFricative { get; } =
        new("β", Voicing.Voiced, Place.Bilabial, Manner.Fricative);

    /// <summary>f, the voiceless labiodental fricative.</summary>
    public static Consonant VoicelessLabiodentalFricative { get; } =
        new("f", Voicing.Voiceless, Place.Labiodental, Manner.Fricative);

    /// <summary>v, the voiced labiodental fricative.</summary>
    public static Consonant VoicedLabiodentalFricative { get; } =
        new("v", Voicing.Voiced, Place.Labiodental, Manner.Fricative);

    /// <summary>θ, the voiceless dental fricative.</summary>
    public static Consonant VoicelessDentalFricative { get; } =
        new("θ", Voicing.Voiceless, Place.Dental, Manner.Fricative);

    /// <summary>ð, the voiced dental fricative.</summary>
    public static Consonant VoicedDentalFricative { get; } =
        new("ð", Voicing.Voiced, Place.Dental, Manner.Fricative);

    /// <summary>s, the voiceless alveolar fricative.</summary>
    public static Consonant VoicelessAlveolarFricative { get; } =
        new("s", Voicing.Voiceless, Place.Alveolar, Manner.Fricative);

    /// <summary>z, the voiced alveolar fricative.</summary>
    public static Consonant VoicedAlveolarFricative { get; } =
        new("z", Voicing.Voiced, Place.Alveolar, Manner.Fricative);

    /// <summary>ʃ, the voiceless postalveolar fricative.</summary>
    public static Consonant VoicelessPostalveolarFricative { get; } =
        new("ʃ", Voicing.Voiceless, Place.Postalveolar, Manner.Fricative);

    /// <summary>ʒ, the voiced postalveolar fricative.</summary>
    public static Consonant VoicedPostalveolarFricative { get; } =
        new("ʒ", Voicing.Voiced, Place.Postalveolar, Manner.Fricative);

    /// <summary>ʂ, the voiceless retroflex fricative.</summary>
    public static Consonant VoicelessRetroflexFricative { get; } =
        new("ʂ", Voicing.Voiceless, Place.Retroflex, Manner.Fricative);

    /// <summary>ʐ, the voiced retroflex fricative.</summary>
    public static Consonant VoicedRetroflexFricative { get; } =
        new("ʐ", Voicing.Voiced, Place.Retroflex, Manner.Fricative);

    /// <summary>ç, the voiceless palatal fricative.</summary>
    public static Consonant VoicelessPalatalFricative { get; } =
        new("ç", Voicing.Voiceless, Place.Palatal, Manner.Fricative);

    /// <summary>ʝ, the voiced palatal fricative.</summary>
    public static Consonant VoicedPalatalFricative { get; } =
        new("ʝ", Voicing.Voiced, Place.Palatal, Manner.Fricative);

    /// <summary>x, the voiceless velar fricative.</summary>
    public static Consonant VoicelessVelarFricative { get; } =
        new("x", Voicing.Voiceless, Place.Velar, Manner.Fricative);

    /// <summary>ɣ, the voiced velar fricative.</summary>
    public static Consonant VoicedVelarFricative { get; } =
        new("ɣ", Voicing.Voiced, Place.Velar, Manner.Fricative);

    /// <summary>χ, the voiceless uvular fricative.</summary>
    public static Consonant VoicelessUvularFricative { get; } =
        new("χ", Voicing.Voiceless, Place.Uvular, Manner.Fricative);

    /// <summary>ʁ, the voiced uvular fricative.</summary>
    public static Consonant VoicedUvularFricative { get; } =
        new("ʁ", Voicing.Voiced, Place.Uvular, Manner.Fricative);

    /// <summary>ħ, the voiceless pharyngeal fricative.</summary>
    public static Consonant VoicelessPharyngealFricative { get; } =
        new("ħ", Voicing.Voiceless, Place.Pharyngeal, Manner.Fricative);

    /// <summary>ʕ, the voiced pharyngeal fricative.</summary>
    public static Consonant VoicedPharyngealFricative { get; } =
        new("ʕ", Voicing.Voiced, Place.Pharyngeal, Manner.Fricative);

    /// <summary>h, the voiceless glottal fricative.</summary>
    public static Consonant VoicelessGlottalFricative { get; } =
        new("h", Voicing.Voiceless, Place.Glottal, Manner.Fricative);

    /// <summary>ɦ, the voiced glottal fricative.</summary>
    public static Consonant VoicedGlottalFricative { get; } =
        new("ɦ", Voicing.Voiced, Place.Glottal, Manner.Fricative);

    /// <summary>ɬ, the voiceless alveolar lateral fricative.</summary>
    public static Consonant VoicelessAlveolarLateralFricative { get; } =
        new("ɬ", Voicing.Voiceless, Place.Alveolar, Manner.LateralFricative);

    /// <summary>ɮ, the voiced alveolar lateral fricative.</summary>
    public static Consonant VoicedAlveolarLateralFricative { get; } =
        new("ɮ", Voicing.Voiced, Place.Alveolar, Manner.LateralFricative);

    /// <summary>ʋ, the voiced labiodental approximant.</summary>
    public static Consonant VoicedLabiodentalApproximant { get; } =
        new("ʋ", Voicing.Voiced, Place.Labiodental, Manner.Approximant);

    /// <summary>ɹ, the voiced alveolar approximant.</summary>
    public static Consonant VoicedAlveolarApproximant { get; } =
        new("ɹ", Voicing.Voiced, Place.Alveolar, Manner.Approximant);

    /// <summary>ɻ, the voiced retroflex approximant.</summary>
    public static Consonant VoicedRetroflexApproximant { get; } =
        new("ɻ", Voicing.Voiced, Place.Retroflex, Manner.Approximant);

    /// <summary>j, the voiced palatal approximant.</summary>
    public static Consonant VoicedPalatalApproximant { get; } =
        new("j", Voicing.Voiced, Place.Palatal, Manner.Approximant);

    /// <summary>ɰ, the voiced velar approximant.</summary>
    public static Consonant VoicedVelarApproximant { get; } =
        new("ɰ", Voicing.Voiced, Place.Velar, Manner.Approximant);

    /// <summary>l, the voiced alveolar lateral approximant.</summary>
    public static Consonant VoicedAlveolarLateralApproximant { get; } =
        new("l", Voicing.Voiced, Place.Alveolar, Manner.LateralApproximant);

    /// <summary>ɭ, the voiced retroflex lateral approximant.</summary>
    public static Consonant VoicedRetroflexLateralApproximant { get; } =
        new("ɭ", Voicing.Voiced, Place.Retroflex, Manner.LateralApproximant);

    /// <summary>ʎ, the voiced palatal lateral approximant.</summary>
    public static Consonant VoicedPalatalLateralApproximant { get; } =
        new("ʎ", Voicing.Voiced, Place.Palatal, Manner.LateralApproximant);

    /// <summary>ʟ, the voiced velar lateral approximant.</summary>
    public static Consonant VoicedVelarLateralApproximant { get; } =
        new("ʟ", Voicing.Voiced, Place.Velar, Manner.LateralApproximant);

    /// <summary>ʍ, the voiceless labial-velar fricative.</summary>
    public static Consonant VoicelessLabialVelarFricative { get; } =
        new("ʍ", Voicing.Voiceless, Place.LabialVelar, Manner.Fricative);

    /// <summary>w, the voiced labial-velar approximant.</summary>
    public static Consonant VoicedLabialVelarApproximant { get; } =
        new("w", Voicing.Voiced, Place.LabialVelar, Manner.Approximant);

    /// <summary>ɥ, the voiced labial-palatal approximant.</summary>
    public static Consonant VoicedLabialPalatalApproximant { get; } =
        new("ɥ", Voicing.Voiced, Place.LabialPalatal, Manner.Approximant);

    /// <summary>ʜ, the voiceless epiglottal fricative.</summary>
    public static Consonant VoicelessEpiglottalFricative { get; } =
        new("ʜ", Voicing.Voiceless, Place.Epiglottal, Manner.Fricative);

    /// <summary>ʢ, the voiced epiglottal fricative.</summary>
    public static Consonant VoicedEpiglottalFricative { get; } =
        new("ʢ", Voicing.Voiced, Place.Epiglottal, Manner.Fricative);

    /// <summary>ʡ, the epiglottal plosive, voiceless as the glottal stop is.</summary>
    public static Consonant VoicelessEpiglottalPlosive { get; } =
        new("ʡ", Voicing.Voiceless, Place.Epiglottal, Manner.Plosive);

    /// <summary>ɕ, the voiceless alveolo-palatal fricative.</summary>
    public static Consonant VoicelessAlveoloPalatalFricative { get; } =
        new("ɕ", Voicing.Voiceless, Place.AlveoloPalatal, Manner.Fricative);

    /// <summary>ʑ, the voiced alveolo-palatal fricative.</summary>
    public static Consonant VoicedAlveoloPalatalFricative { get; } =
        new("ʑ", Voicing.Voiced, Place.AlveoloPalatal, Manner.Fricative);

    /// <summary>ɺ, the voiced alveolar lateral flap.</summary>
    public static Consonant VoicedAlveolarLateralFlap { get; } =
        new("ɺ", Voicing.Voiced, Place.Alveolar, Manner.LateralFlap);

    /// <summary>t͡s, the voiceless alveolar affricate, t released into s.</summary>
    public static Consonant VoicelessAlveolarAffricate { get; } =
        Consonant.Affricate(VoicelessAlveolarPlosive, VoicelessAlveolarFricative);

    /// <summary>d͡z, the voiced alveolar affricate, d released into z.</summary>
    public static Consonant VoicedAlveolarAffricate { get; } =
        Consonant.Affricate(VoicedAlveolarPlosive, VoicedAlveolarFricative);

    /// <summary>t͡ʃ, the voiceless postalveolar affricate, t released into ʃ.</summary>
    public static Consonant VoicelessPostalveolarAffricate { get; } =
        Consonant.Affricate(VoicelessAlveolarPlosive, VoicelessPostalveolarFricative);

    /// <summary>d͡ʒ, the voiced postalveolar affricate, d released into ʒ.</summary>
    public static Consonant VoicedPostalveolarAffricate { get; } =
        Consonant.Affricate(VoicedAlveolarPlosive, VoicedPostalveolarFricative);

    /// <summary>The pulmonic chart's 59 consonants, row by row and, within a row, from the lips back, voiceless first.</summary>
    public static ImmutableArray<Consonant> PulmonicChart { get; } =
    [
        VoicelessBilabialPlosive, VoicedBilabialPlosive, VoicelessAlveolarPlosive,
        VoicedAlveolarPlosive, VoicelessRetroflexPlosive, VoicedRetroflexPlosive,
        VoicelessPalatalPlosive, VoicedPalatalPlosive, VoicelessVelarPlosive, VoicedVelarPlosive,
        VoicelessUvularPlosive, VoicedUvularPlosive,
        VoicelessGlottalPlosive,
        VoicedBilabialNasal, VoicedLabiodentalNasal, VoicedAlveolarNasal, VoicedRetroflexNasal,
        VoicedPalatalNasal, VoicedVelarNasal, VoicedUvularNasal,
        VoicedBilabialTrill, VoicedAlveolarTrill, VoicedUvularTrill,
        VoicedLabiodentalFlap, VoicedAlveolarTap, VoicedRetroflexFlap,
        VoicelessBilabialFricative, VoicedBilabialFricative, VoicelessLabiodentalFricative,
        VoicedLabiodentalFricative, VoicelessDentalFricative, VoicedDentalFricative,
        VoicelessAlveolarFricative, VoicedAlveolarFricative, VoicelessPostalveolarFricative,
        VoicedPostalveolarFricative, VoicelessRetroflexFricative, VoicedRetroflexFricative,
        VoicelessPalatalFricative, VoicedPalatalFricative, VoicelessVelarFricative,
        VoicedVelarFricative,
        VoicelessUvularFricative, VoicedUvularFricative, VoicelessPharyngealFricative,
        VoicedPharyngealFricative, VoicelessGlottalFricative, VoicedGlottalFricative,
        VoicelessAlveolarLateralFricative, VoicedAlveolarLateralFricative,
        VoicedLabiodentalApproximant, VoicedAlveolarApproximant, VoicedRetroflexApproximant,
        VoicedPalatalApproximant, VoicedVelarApproximant,
        VoicedAlveolarLateralApproximant, VoicedRetroflexLateralApproximant,
        VoicedPalatalLateralApproximant, VoicedVelarLateralApproximant,
    ];

    /// <summary>The chart's other consonants: the labial-velars, the labial-palatal, the epiglottals, the alveolo-palatals and the lateral flap.</summary>
    public static ImmutableArray<Consonant> OtherSymbols { get; } =
    [
        VoicelessLabialVelarFricative, VoicedLabialVelarApproximant, VoicedLabialPalatalApproximant,
        VoicelessEpiglottalFricative, VoicedEpiglottalFricative, VoicelessEpiglottalPlosive,
        VoicelessAlveoloPalatalFricative, VoicedAlveoloPalatalFricative, VoicedAlveolarLateralFlap,
    ];

    /// <summary>The common affricates, t͡s, d͡z, t͡ʃ and d͡ʒ.</summary>
    public static ImmutableArray<Consonant> Affricates { get; } =
    [
        VoicelessAlveolarAffricate, VoicedAlveolarAffricate,
        VoicelessPostalveolarAffricate, VoicedPostalveolarAffricate,
    ];

    /// <summary>Every consonant named here: the pulmonic chart, then the other symbols, then the affricates.</summary>
    public static ImmutableArray<Consonant> All { get; } =
        [.. PulmonicChart, .. OtherSymbols, .. Affricates];
}
