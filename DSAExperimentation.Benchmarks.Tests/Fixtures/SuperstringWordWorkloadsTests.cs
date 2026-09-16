using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for SuperstringWordWorkloads (ARCHITECTURE 17.7): LC 943 orders words, so the
// fixture has to hand it words that can genuinely overlap. One fixed word length is what keeps the
// problem's own "no word is a substring of another" premise true without a filter pass, and the
// DNA-sized alphabet is what makes suffix/prefix overlaps common rather than coincidental.
public sealed partial class SuperstringWordWorkloadsTests
{
    private const int WordCount = 64;
    private const int WordLength = 12;
    private const int Seed = 943; // LC problem number
    private const string Alphabet = "ACGT";

    [Fact]
    public void BuildWords_WordCountAndLength_ReturnsThatManyDistinctWordsOfThatLength()
    {
        var words = SuperstringWordWorkloads.BuildWords(WordCount, WordLength, Seed);

        Assert.Equal(WordCount, words.Length);
        Assert.Equal(WordCount, words.Distinct().Count());
        Assert.All(words, word => Assert.Equal(WordLength, word.Length));
    }

    [Fact]
    public void BuildWords_EveryPosition_IsDrawnFromTheDnaAlphabet() =>
        Assert.All(
            SuperstringWordWorkloads.BuildWords(WordCount, WordLength, Seed),
            word => Assert.All(word, letter => Assert.True(Alphabet.Contains(letter))));

    // Distinct words of one fixed length cannot contain one another, which is exactly the premise
    // LC 943's overlap search is stated under, so the ordering search never sees a degenerate pair.
    [Fact]
    public void BuildWords_EqualLengthWords_LeaveNoWordInsideAnother()
    {
        var words = SuperstringWordWorkloads.BuildWords(WordCount, WordLength, Seed);

        Assert.All(
            words,
            word => Assert.DoesNotContain(
                words,
                other => other != word && other.Contains(word, StringComparison.Ordinal)));
    }

    [Fact]
    public void BuildWords_SameSeed_ReturnsTheSameWords() =>
        Assert.Equal(
            SuperstringWordWorkloads.BuildWords(WordCount, WordLength, Seed),
            SuperstringWordWorkloads.BuildWords(WordCount, WordLength, Seed));
}
