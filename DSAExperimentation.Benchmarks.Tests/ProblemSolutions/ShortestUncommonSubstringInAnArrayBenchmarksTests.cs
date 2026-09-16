using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortestUncommonSubstringInAnArrayBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - enumerating each word's candidate
// substrings against an Aho-Corasick automaton over the whole array - so a harness whose arms
// disagree is resolving two different arrays. Setup draws the words from one fixed seed over a
// narrow alphabet, so the same WordCount must rebuild the same array; otherwise two published
// numbers were never comparable.
//
// The answer is one shortest uncommon substring per input word, and entry i belongs to word i,
// so AnswerText.Of rather than OfUnorderedSet keeps each substring scored against its own word.
public sealed partial class ShortestUncommonSubstringInAnArrayBenchmarksTests
{
    private const int SmallestWordCount = 10;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWords() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_FourLetterAlphabetWords_AgreesWithAhoCorasick()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.AhoCorasick()),
            AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void AhoCorasick_FourLetterAlphabetWords_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForce()),
            AnswerText.Of(harness.AhoCorasick()));
    }

    private static ShortestUncommonSubstringInAnArrayBenchmarks BuildHarness()
    {
        var harness = new ShortestUncommonSubstringInAnArrayBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
