using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordsWithinTwoEditsOfDictionaryBenchmarks (ARCHITECTURE 17.9): both arms are
// competing matchers for one question, so a harness whose arms disagree is timing two different
// problems. LeetCode's own answer is the matching queries in the order they were given, which is the
// outer order the problem pins - so the comparison is order-sensitive, not a set. The workload makes
// every query a genuine match (each is one dictionary word with 0, 1, or 2 characters changed), so
// the decisive value is that the answer holds every query.
public sealed partial class WordsWithinTwoEditsOfDictionaryBenchmarksTests
{
    private const int SmallestDictionarySize = 2_000;
    private const int ExpectedMatchCount = SmallestDictionarySize;

    [Fact]
    public void Setup_SameDictionarySize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_AgreesWithTrieSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForce()), AnswerText.Of(harness.TrieSearch()));
    }

    [Fact]
    public void TrieSearch_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.TrieSearch()), AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void BruteForce_EveryQueryWithinTheEditBudget_Matches() =>
        Assert.Equal(ExpectedMatchCount, BuildHarness().BruteForce().Length);

    [Fact]
    public void TrieSearch_EveryQueryWithinTheEditBudget_Matches() =>
        Assert.Equal(ExpectedMatchCount, BuildHarness().TrieSearch().Length);

    private static WordsWithinTwoEditsOfDictionaryBenchmarks BuildHarness()
    {
        var harness = new WordsWithinTwoEditsOfDictionaryBenchmarks { DictionarySize = SmallestDictionarySize };
        harness.Setup();

        return harness;
    }
}
