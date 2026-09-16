using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordSearchIIBenchmarks (ARCHITECTURE 17.9): both arms are competing search
// strategies for one question - which of the words are on the board - so a harness whose arms
// disagree is timing two different problems. Note what the arms agree ON: each returns the COUNT of
// the found words, not the words themselves, so agreement witnesses the same number of matches and
// cannot witness the same matches. The board and the word list are drawn from one seeded generator
// and the words are random, so there is no independently derivable expected count here; the
// agreement below is as strong as that workload allows, and no stronger.
public sealed partial class WordSearchIIBenchmarksTests
{
    private const int SmallestWordCount = 20;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().PerWordBruteForce()),
            AnswerText.Of(BuildHarness().PerWordBruteForce()));

    [Fact]
    public void PerWordBruteForce_AgreesWithTriePrunedSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PerWordBruteForce(), harness.TriePrunedSearch());
    }

    [Fact]
    public void TriePrunedSearch_AgreesWithPerWordBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TriePrunedSearch(), harness.PerWordBruteForce());
    }

    private static WordSearchIIBenchmarks BuildHarness()
    {
        var harness = new WordSearchIIBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
