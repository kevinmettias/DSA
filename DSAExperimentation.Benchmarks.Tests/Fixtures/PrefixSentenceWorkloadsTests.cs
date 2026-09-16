using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for PrefixSentenceWorkloads (ARCHITECTURE 17.7). The reading depends on LC 1455's
// sentence being LeetCode's own one-string shape - space-separated words - so the scan has separators to
// walk rather than a single token, which is the case the search key is a whole word.
public sealed partial class PrefixSentenceWorkloadsTests
{
    private const int WordCount = 200;
    private const int Seed = 1455; // LC problem number
    private const char WordSeparator = ' ';
    private const int MinWordLength = 3;
    private const int MaxWordLengthExclusive = 8;
    private const char FirstLetter = 'a';
    private const int AlphabetSize = 26;

    [Fact]
    public void BuildSentence_WordCount_ReturnsExactlyThatManySpaceSeparatedWords() =>
        Assert.Equal(
            WordCount,
            PrefixSentenceWorkloads.BuildSentence(WordCount, Seed).Split(WordSeparator).Length);

    [Fact]
    public void BuildSentence_EveryWord_IsARunOfOneRepeatedLetterWithinTheDocumentedLengthBand()
    {
        var words = PrefixSentenceWorkloads.BuildSentence(WordCount, Seed).Split(WordSeparator);

        Assert.All(words, word => Assert.InRange(word.Length, MinWordLength, MaxWordLengthExclusive - 1));
        Assert.All(words, word => Assert.Single(word.Distinct()));
        Assert.All(
            words,
            word => Assert.All(
                word,
                letter => Assert.InRange(letter, FirstLetter, (char)(FirstLetter + AlphabetSize - 1))));
    }

    [Fact]
    public void BuildSentence_SameSeed_ReturnsTheSameSentence() =>
        Assert.Equal(
            PrefixSentenceWorkloads.BuildSentence(WordCount, Seed),
            PrefixSentenceWorkloads.BuildSentence(WordCount, Seed));
}
