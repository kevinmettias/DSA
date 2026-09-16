using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for RandomWords (ARCHITECTURE 17.7). The reading depends on words being drawn from a
// deliberately narrowed alphabet - over the full 26 letters almost every candidate pair diverges on its
// first or last character, so no strategy gets to walk a meaningful span - which stays the caller's
// choice, and on the two overloads being one workload with and without a caller-supplied stream.
public sealed partial class RandomWordsTests
{
    private const int WordCount = 64;
    private const int MaxLength = 8;
    private const int Seed = 3042;
    private const int ShortestWordLength = 1;
    private const string Alphabet = "abcd";

    [Fact]
    public void Build_WordCount_ReturnsOneWordPerPosition() =>
        Assert.Equal(WordCount, RandomWords.Build(WordCount, Alphabet, MaxLength, Seed).Length);

    [Fact]
    public void Build_EveryWord_IsNonEmptyWithinTheMaximumLengthAndDrawnFromTheSuppliedAlphabet()
    {
        var words = RandomWords.Build(WordCount, Alphabet, MaxLength, Seed);

        Assert.All(words, word => Assert.InRange(word.Length, ShortestWordLength, MaxLength));
        Assert.All(words.SelectMany(word => word), letter => Assert.Contains(letter, Alphabet));
    }

    // A draw that gave every word the same length would leave the maximum untested and hide the length
    // rule behind a constant, so the spread across the accepted band is asserted directly.
    [Fact]
    public void Build_WordLengths_SpreadAcrossTheAcceptedBand() =>
        Assert.NotEqual(
            RandomWords.Build(WordCount, Alphabet, MaxLength, Seed).Min(word => word.Length),
            RandomWords.Build(WordCount, Alphabet, MaxLength, Seed).Max(word => word.Length));

    // The seed overload is a promise about the Random overload: it must be the same draw with a fresh
    // stream, or a caller switching between them silently changes the workload it measures.
    [Fact]
    public void Build_SeedOverload_AgreesWithTheRandomOverloadOnTheSameSeed() =>
        Assert.Equal(
            AnswerText.Of(RandomWords.Build(WordCount, Alphabet, MaxLength, new Random(Seed))),
            AnswerText.Of(RandomWords.Build(WordCount, Alphabet, MaxLength, Seed)));

    [Fact]
    public void Build_RandomOverload_SameSeed_ReturnsTheSameWords() =>
        Assert.Equal(
            AnswerText.Of(RandomWords.Build(WordCount, Alphabet, MaxLength, new Random(Seed))),
            AnswerText.Of(RandomWords.Build(WordCount, Alphabet, MaxLength, new Random(Seed))));
}
