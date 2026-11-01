using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignAddAndSearchWordsDataStructureBenchmarks (ARCHITECTURE 17.9): the generic
// arm check compares the arms only with each other, so what stays here is a bound derived without either
// arm. Setup builds one word list and one search pattern per word, reading the pattern off an added word
// for every second entry, so at least half the searches are guaranteed hits; the reading is the number of
// matching searches.
public sealed partial class DesignAddAndSearchWordsDataStructureBenchmarksTests
{
    private const int SmallestWordCount = 200;

    // Setup reads every second pattern off a word it added, so those searches match; none is searched
    // twice, so the count can never exceed the pattern count.
    private const int MinimumMatchCount = SmallestWordCount / 2;
    private const int MaximumMatchCount = SmallestWordCount;

    [Fact]
    public void LinearScan_TwoHundredSeededWords_MatchesAtLeastEveryExactLookup() =>
        Assert.InRange(BuildHarness().LinearScan(), MinimumMatchCount, MaximumMatchCount);

    private static DesignAddAndSearchWordsDataStructureBenchmarks BuildHarness()
    {
        var harness = new DesignAddAndSearchWordsDataStructureBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
