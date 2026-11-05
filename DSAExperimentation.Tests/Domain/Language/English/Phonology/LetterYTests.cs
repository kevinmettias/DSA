using DSAExperimentation.Domain.Language.English.Orthography;
using DSAExperimentation.Domain.Language.English.Phonology;
using DSAExperimentation.Domain.Language.Phonetics;

namespace DSAExperimentation.Tests.Domain.Language.English.Phonology;

// y's two roles, each checked where it lives: in spelling, a consonant letter and no written vowel;
// in sound, a consonant of the accent's and two of its vowels, which a transcription reads back.
public sealed partial class LetterYTests
{
    [Fact]
    public void Letter_IsAConsonantLetter() => Assert.True(EnglishConsonants.Has(LetterY.Letter));

    [Fact]
    public void Letter_IsNoWrittenVowel() => Assert.False(EnglishVowels.Has(LetterY.Letter));

    [Fact]
    public void ConsonantSound_IsThePalatalApproximant() => Assert.Equal("j", LetterY.ConsonantSound.Symbol);

    [Fact]
    public void ConsonantSound_IsAConsonantOfTheAccent() =>
        Assert.Contains(LetterY.ConsonantSound, ReceivedPronunciation.Consonants);

    // "yes", /jes/, begins with the consonant y spells.
    [Fact]
    public void ConsonantSound_BeginsYes() =>
        Assert.Equal(LetterY.ConsonantSound.Symbol, PhonemicTranscription<ReceivedPronunciation>.Segment("jes")[0]);

    [Fact]
    public void SingleVowelSound_IsKit() => Assert.Equal(ReceivedPronunciation.Kit, LetterY.SingleVowelSound);

    // "gym", /d͡ʒɪm/, holds the single vowel y spells.
    [Fact]
    public void SingleVowelSound_IsTheVowelOfGym() =>
        Assert.Equal(LetterY.SingleVowelSound.Symbol, PhonemicTranscription<ReceivedPronunciation>.Segment("d͡ʒɪm")[1]);

    [Fact]
    public void DiphthongSound_IsPrice() => Assert.Equal(ReceivedPronunciation.Price, LetterY.DiphthongSound);

    // "my", /maɪ/, ends in the diphthong y spells.
    [Fact]
    public void DiphthongSound_EndsMy() =>
        Assert.Equal(LetterY.DiphthongSound.Symbol, PhonemicTranscription<ReceivedPronunciation>.Segment("maɪ")[^1]);
}
