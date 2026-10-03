using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordBreakIIBenchmarks (ARCHITECTURE 17.9): both arms are competing
// segmentation strategies for one source and one dictionary, so a harness whose arms disagree is
// timing two different problems. Each arm returns the sentences themselves. The workload is a
// source tiling one dictionary word, whose only segmentation is that tiling - one sentence, the
// word repeated with single spaces between - which the assertions below pin, so the comparison is
// anchored to something independent of both arms.
public sealed partial class WordBreakIIBenchmarksTests
{
    private const int SmallestLength = 600;
    private const int ExpectedSentenceCount = 1;

    // Restated from the benchmark: the one dictionary word the source tiles.
    private const string RepeatedWord = "cat";

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerGraphText.Of(BuildHarness().HashSetUnboundedScan()),
            AnswerGraphText.Of(BuildHarness().HashSetUnboundedScan()));

    [Fact]
    public void HashSetUnboundedScan_SingleDictionaryWord_FindsOneSentence() =>
        AssertTheTilingSentence(BuildHarness().HashSetUnboundedScan());

    [Fact]
    public void TriePrunedMemoized_SingleDictionaryWord_FindsOneSentence() =>
        AssertTheTilingSentence(BuildHarness().TriePrunedMemoized());

    private static void AssertTheTilingSentence(List<string> sentences)
    {
        Assert.Equal(ExpectedSentenceCount, sentences.Count);
        Assert.Equal(string.Join(' ', Enumerable.Repeat(RepeatedWord, SmallestLength / RepeatedWord.Length)), sentences[0]);
    }

    private static WordBreakIIBenchmarks BuildHarness()
    {
        var harness = new WordBreakIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
