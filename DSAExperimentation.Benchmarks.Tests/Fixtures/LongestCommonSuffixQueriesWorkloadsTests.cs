using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for LongestCommonSuffixQueriesWorkloads (ARCHITECTURE 17.7). The reading depends on
// LC 3093's container and query words being drawn over a deliberately small alphabet, so they share
// long suffixes, and on the two lists coming from one random stream rather than two identical ones.
public sealed partial class LongestCommonSuffixQueriesWorkloadsTests
{
    private const int Count = 64;
    private const int MaxWordLength = 10;
    private const int Seed = 3093; // LC problem number
    private const char FirstAlphabetLetter = 'a';
    private const char LastAlphabetLetter = 'b';
    private const int MinWordLength = 1;

    [Fact]
    public void BuildWords_Count_ReturnsOneWordPerRequestedPosition() =>
        Assert.Equal(Count, LongestCommonSuffixQueriesWorkloads.BuildWords(Count, new Random(Seed)).Length);

    [Fact]
    public void BuildWords_EveryWord_IsANonEmptyRunOverTheSmallAlphabet()
    {
        var words = LongestCommonSuffixQueriesWorkloads.BuildWords(Count, new Random(Seed));

        Assert.All(words, word => Assert.InRange(word.Length, MinWordLength, MaxWordLength));
        Assert.All(
            words,
            word => Assert.All(
                word,
                character => Assert.InRange(character, FirstAlphabetLetter, LastAlphabetLetter)));
    }

    // This generator takes a Random rather than a seed precisely so the harness can draw both lists
    // from one stream: a second list drawn from the same stream must differ from the first, which is
    // what a per-list Random would have made identical every run.
    [Fact]
    public void BuildWords_ListDrawnAfterAnother_IsNotTheSameList()
    {
        var random = new Random(Seed);
        var containerWords = LongestCommonSuffixQueriesWorkloads.BuildWords(Count, random);
        var queryWords = LongestCommonSuffixQueriesWorkloads.BuildWords(Count, random);

        Assert.NotEqual(AnswerText.Of(containerWords), AnswerText.Of(queryWords));
    }

    [Fact]
    public void BuildWords_TwoRandomsFromTheSameSeed_ReturnTheSameWords() =>
        Assert.Equal(
            LongestCommonSuffixQueriesWorkloads.BuildWords(Count, new Random(Seed)),
            LongestCommonSuffixQueriesWorkloads.BuildWords(Count, new Random(Seed)));
}
