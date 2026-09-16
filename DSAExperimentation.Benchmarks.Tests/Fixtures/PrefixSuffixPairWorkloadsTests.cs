using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for PrefixSuffixPairWorkloads (ARCHITECTURE 17.7). The reading depends on LC 3042's
// words coming from a narrowed four-letter alphabet: over the full 26 letters almost every candidate
// pair diverges on its first or last character, letting both strategies short-circuit instead of walking
// a meaningful prefix/suffix span.
public sealed partial class PrefixSuffixPairWorkloadsTests
{
    private const int WordCount = 100;
    private const int MaxLength = 30;
    private const int Seed = 3045;
    private const int ShortestWordLength = 1;
    private const string Alphabet = "abcd";

    [Fact]
    public void BuildWords_WordCount_ReturnsOneWordPerPosition() =>
        Assert.Equal(
            WordCount,
            PrefixSuffixPairWorkloads.BuildWords(WordCount, MaxLength, Seed).Length);

    [Fact]
    public void BuildWords_EveryWord_IsNonEmptyWithinTheMaximumLengthAndDrawnFromTheFourLetterAlphabet() =>
        Assert.All(
            PrefixSuffixPairWorkloads.BuildWords(WordCount, MaxLength, Seed),
            word => Assert.InRange(word.Length, ShortestWordLength, MaxLength));

    [Fact]
    public void BuildWords_EveryCharacter_ComesFromTheNarrowedAlphabet() =>
        Assert.All(
            PrefixSuffixPairWorkloads.BuildWords(WordCount, MaxLength, Seed).SelectMany(word => word),
            letter => Assert.Contains(letter, Alphabet));

    // The two entries are the same workload through two names - this one names the alphabet that makes
    // prefix/suffix matches actually occur, the shared generator takes an alphabet as an argument - so a
    // drift between them is a workload change for LC 3042 only, which the other callers would not show.
    [Fact]
    public void BuildWords_AgreesWithTheSharedGeneratorOnTheSameNarrowedAlphabet() =>
        Assert.Equal(
            AnswerText.Of(RandomWords.Build(WordCount, Alphabet, MaxLength, Seed)),
            AnswerText.Of(PrefixSuffixPairWorkloads.BuildWords(WordCount, MaxLength, Seed)));

    [Fact]
    public void BuildWords_SameSeed_ReturnsTheSameWords() =>
        Assert.Equal(
            AnswerText.Of(PrefixSuffixPairWorkloads.BuildWords(WordCount, MaxLength, Seed)),
            AnswerText.Of(PrefixSuffixPairWorkloads.BuildWords(WordCount, MaxLength, Seed)));
}
