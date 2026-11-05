using DSAExperimentation.Domain.Language.Phonetics.Consonants;

namespace DSAExperimentation.Tests.Domain.Language.Phonetics.Consonants;

// An affricate is a plosive released into a fricative voiced alike, written as the two symbols
// joined by the tie bar and made where the fricative is.
public sealed partial class ConsonantTests
{
    public static TheoryData<Consonant, Consonant> NoAffricate => new()
    {
        { IpaConsonants.VoicelessAlveolarFricative, IpaConsonants.VoicelessPostalveolarFricative },
        { IpaConsonants.VoicelessAlveolarPlosive, IpaConsonants.VoicedAlveolarNasal },
        { IpaConsonants.VoicelessAlveolarPlosive, IpaConsonants.VoicedPostalveolarFricative },
    };

    [Fact]
    public void Affricate_StopAndFricative_JoinsTheirSymbolsWithTheTieBar() =>
        Assert.Equal(
            "t͡ʃ",
            Consonant.Affricate(IpaConsonants.VoicelessAlveolarPlosive, IpaConsonants.VoicelessPostalveolarFricative).Symbol);

    [Fact]
    public void Affricate_StopAndFricative_IsMadeWhereTheFricativeIs() =>
        Assert.Equal(
            new Consonant("d͡ʒ", Voicing.Voiced, Place.Postalveolar, Manner.Affricate),
            Consonant.Affricate(IpaConsonants.VoicedAlveolarPlosive, IpaConsonants.VoicedPostalveolarFricative));

    [Theory]
    [MemberData(nameof(NoAffricate))]
    public void Affricate_NotAPlosiveIntoAFricativeVoicedAlike_Throws(Consonant stop, Consonant fricative) =>
        Assert.Throws<ArgumentException>(() => Consonant.Affricate(stop, fricative));
}
