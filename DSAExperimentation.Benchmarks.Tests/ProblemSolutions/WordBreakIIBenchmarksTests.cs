using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordBreakIIBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms
// to agreeing with each other, which a shared wrong answer would pass. Each arm returns the
// sentences themselves. The workload is a source tiling one dictionary word, whose only
// segmentation is that tiling - one sentence, the word repeated with single spaces between - which
// the assertion below pins against the baseline, anchoring it to something independent of both arms.
public sealed partial class WordBreakIIBenchmarksTests
{
    private const int SmallestLength = 6;

    // Restated from the benchmark: the one dictionary word the source tiles.
    private const string RepeatedWord = "cat";

    [Fact]
    public void HashSetUnboundedScan_SingleDictionaryWord_FindsOneSentence() =>
        AssertTheTilingSentence(BuildHarness().HashSetUnboundedScan());

    private static void AssertTheTilingSentence(List<string> sentences)
    {
        var sentence = Assert.Single(sentences);
        Assert.Equal(string.Join(' ', Enumerable.Repeat(RepeatedWord, SmallestLength / RepeatedWord.Length)), sentence);
    }

    private static WordBreakIIBenchmarks BuildHarness()
    {
        var harness = new WordBreakIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
