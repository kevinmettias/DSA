using DSAExperimentation.Domain.Language.Phonetics.Consonants;

namespace DSAExperimentation.Tests.Domain.Language.Phonetics.Consonants;

// The chart is the fact itself, so the tests spell it out row by row, check that no two symbols
// name one sound, and hold some named values to the IPA's own names.
public sealed partial class IpaConsonantsTests
{
    private const int IpaScriptG = 0x0261;

    public static TheoryData<Consonant, Consonant> NamedConsonants => new()
    {
        { IpaConsonants.VoicelessBilabialPlosive, new("p", Voicing.Voiceless, Place.Bilabial, Manner.Plosive) },
        { IpaConsonants.VoicelessGlottalPlosive, new("ʔ", Voicing.Voiceless, Place.Glottal, Manner.Plosive) },
        { IpaConsonants.VoicedVelarNasal, new("ŋ", Voicing.Voiced, Place.Velar, Manner.Nasal) },
        { IpaConsonants.VoicedDentalFricative, new("ð", Voicing.Voiced, Place.Dental, Manner.Fricative) },
        {
            IpaConsonants.VoicelessPostalveolarFricative,
            new("ʃ", Voicing.Voiceless, Place.Postalveolar, Manner.Fricative)
        },
        { IpaConsonants.VoicedAlveolarApproximant, new("ɹ", Voicing.Voiced, Place.Alveolar, Manner.Approximant) },
        { IpaConsonants.VoicedLabialVelarApproximant, new("w", Voicing.Voiced, Place.LabialVelar, Manner.Approximant) },
        { IpaConsonants.VoicedAlveolarLateralFlap, new("ɺ", Voicing.Voiced, Place.Alveolar, Manner.LateralFlap) },
    };

    [Theory]
    [MemberData(nameof(NamedConsonants))]
    public void NamedConsonant_IsItsSymbolAndArticulation(Consonant consonant, Consonant expected) =>
        Assert.Equal(expected, consonant);

    [Fact]
    public void PulmonicChart_SpellsTheChartRowByRow() =>
        Assert.Equal(
            "pbtdʈɖcɟkɡqɢʔ" + "mɱnɳɲŋɴ" + "ʙrʀ" + "ⱱɾɽ" + "ɸβfvθðszʃʒʂʐçʝxɣχʁħʕhɦ" + "ɬɮ" + "ʋɹɻjɰ" + "lɭʎʟ",
            string.Concat(IpaConsonants.PulmonicChart.Select(consonant => consonant.Symbol)));

    [Fact]
    public void PulmonicChart_HoldsFiftyNine() => Assert.Equal(59, IpaConsonants.PulmonicChart.Length);

    [Fact]
    public void OtherSymbols_SpellTheChartsOthers() =>
        Assert.Equal("ʍwɥʜʢʡɕʑɺ", string.Concat(IpaConsonants.OtherSymbols.Select(consonant => consonant.Symbol)));

    [Fact]
    public void Affricates_AreTheFourJoinedByTheTieBar() =>
        Assert.Equal<string>(["t͡s", "d͡z", "t͡ʃ", "d͡ʒ"], IpaConsonants.Affricates.Select(consonant => consonant.Symbol));

    [Fact]
    public void All_IsTheChartThenItsOthersThenTheAffricates() =>
        Assert.Equal<Consonant>(
            [.. IpaConsonants.PulmonicChart, .. IpaConsonants.OtherSymbols, .. IpaConsonants.Affricates],
            IpaConsonants.All);

    [Fact]
    public void All_GivesEachArticulationOneSymbol() =>
        Assert.Equal(
            IpaConsonants.All.Length,
            IpaConsonants.All.Select(consonant => (consonant.Voicing, consonant.Place, consonant.Manner)).Distinct().Count());

    [Fact]
    public void All_WritesEachSymbolOnce() =>
        Assert.Equal(IpaConsonants.All.Length, IpaConsonants.All.Select(consonant => consonant.Symbol).Distinct().Count());

    // The IPA's own letter, not the keyboard's g, which looks the same in many fonts.
    [Fact]
    public void VoicedVelarPlosive_IsWrittenWithTheIpaLetter() =>
        Assert.Equal(IpaScriptG, IpaConsonants.VoicedVelarPlosive.Symbol.Single());
}
