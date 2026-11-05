using DSAExperimentation.Domain.Language.English.Phonology;
using DSAExperimentation.Domain.Language.Phonetics.Consonants;
using DSAExperimentation.Domain.Language.Phonetics.Vowels;

namespace DSAExperimentation.Tests.Domain.Language.English.Phonology;

// The 44 phonemes are the fact itself, so the tests spell each list out in the dictionaries'
// transcription, check every sound is the IPA's, and hold some lexical sets to their vowels.
public sealed partial class ReceivedPronunciationTests
{
    public static TheoryData<Monophthong, string> NamedMonophthongs => new()
    {
        { ReceivedPronunciation.Fleece, "iː" },
        { ReceivedPronunciation.Kit, "ɪ" },
        { ReceivedPronunciation.Dress, "e" },
        { ReceivedPronunciation.Palm, "ɑː" },
        { ReceivedPronunciation.Lot, "ɒ" },
        { ReceivedPronunciation.Thought, "ɔː" },
        { ReceivedPronunciation.Nurse, "ɜː" },
        { ReceivedPronunciation.Comma, "ə" },
    };

    public static TheoryData<Diphthong, string> NamedDiphthongs => new()
    {
        { ReceivedPronunciation.Face, "eɪ" },
        { ReceivedPronunciation.Price, "aɪ" },
        { ReceivedPronunciation.Goat, "əʊ" },
        { ReceivedPronunciation.Square, "eə" },
        { ReceivedPronunciation.Cure, "ʊə" },
    };

    [Theory]
    [MemberData(nameof(NamedMonophthongs))]
    public void NamedMonophthong_IsItsLexicalSetsVowel(Monophthong vowel, string symbol) =>
        Assert.Equal(symbol, vowel.Symbol);

    [Theory]
    [MemberData(nameof(NamedDiphthongs))]
    public void NamedDiphthong_IsItsLexicalSetsVowel(Diphthong vowel, string symbol) =>
        Assert.Equal(symbol, vowel.Symbol);

    [Fact]
    public void Monophthongs_AreTheTwelveSingleVowels() =>
        Assert.Equal<string>(
            ["iː", "ɪ", "e", "æ", "ʌ", "ɑː", "ɒ", "ɔː", "ʊ", "uː", "ɜː", "ə"],
            ReceivedPronunciation.Monophthongs.Select(vowel => vowel.Symbol));

    [Fact]
    public void Monophthongs_EachIsAVowelOfTheIpaChart() =>
        Assert.All(ReceivedPronunciation.Monophthongs, vowel => Assert.Contains(vowel.Quality, IpaVowels.All));

    [Fact]
    public void Diphthongs_AreTheEight() =>
        Assert.Equal<string>(
            ["eɪ", "aɪ", "ɔɪ", "əʊ", "aʊ", "ɪə", "eə", "ʊə"],
            ReceivedPronunciation.Diphthongs.Select(vowel => vowel.Symbol));

    [Fact]
    public void Diphthongs_EachGlidesBetweenVowelsOfTheIpaChart() =>
        Assert.All(
            ReceivedPronunciation.Diphthongs.SelectMany(vowel => new[] { vowel.Start, vowel.End }),
            quality => Assert.Contains(quality, IpaVowels.All));

    [Fact]
    public void Consonants_AreTheTwentyFour() =>
        Assert.Equal<string>(
            ["p", "b", "t", "d", "k", "ɡ", "t͡ʃ", "d͡ʒ", "f", "v", "θ", "ð", "s", "z", "ʃ", "ʒ", "h", "m", "n", "ŋ", "l", "ɹ", "j", "w"],
            ReceivedPronunciation.Consonants.Select(consonant => consonant.Symbol));

    [Fact]
    public void Consonants_EachIsAnIpaConsonant() =>
        Assert.All(ReceivedPronunciation.Consonants, consonant => Assert.Contains(consonant, IpaConsonants.All));

    // English's 44 sounds, as they are usually counted, each written differently.
    [Fact]
    public void Phonemes_AreFortyFourDistinctSymbols() =>
        Assert.Equal(
            44,
            ReceivedPronunciation.Monophthongs.Select(vowel => vowel.Symbol)
                .Concat(ReceivedPronunciation.Diphthongs.Select(vowel => vowel.Symbol))
                .Concat(ReceivedPronunciation.Consonants.Select(consonant => consonant.Symbol))
                .Distinct()
                .Count());
}
