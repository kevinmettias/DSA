using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordsWithinTwoEditsOfDictionaryBenchmarks (ARCHITECTURE 17.9): the generic
// check holds the arms to agreeing with each other, which a shared wrong answer would pass. The
// workload makes every query a genuine match (each is one dictionary word with 0, 1, or 2
// characters changed), so the decisive value, pinned on the baseline, is that the answer holds
// every query.
public sealed partial class WordsWithinTwoEditsOfDictionaryBenchmarksTests
{
    private const int SmallestDictionarySize = 10;
    private const int ExpectedMatchCount = SmallestDictionarySize;

    [Fact]
    public void BruteForce_EveryQueryWithinTheEditBudget_Matches() =>
        Assert.Equal(ExpectedMatchCount, BuildHarness().BruteForce().Length);

    private static WordsWithinTwoEditsOfDictionaryBenchmarks BuildHarness()
    {
        var harness = new WordsWithinTwoEditsOfDictionaryBenchmarks { DictionarySize = SmallestDictionarySize };
        harness.Setup();

        return harness;
    }
}
