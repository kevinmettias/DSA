using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignAddAndSearchWordsDataStructureBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the same search over a linear word list against a trie
// - so a harness whose arms disagree is timing two different problems. Setup builds one word list and one
// search pattern per word from a fixed seed, then reads the pattern off an added word for every second
// entry, so at least half the searches are guaranteed hits and a trie's fast path cannot quietly answer
// fewer of them. Each arm builds its own dictionary inside the call, so a single harness is safe to call
// in either order; the reading is the number of matching searches, and the same WordCount must rebuild the
// same words and patterns and with it the same count.
public sealed partial class DesignAddAndSearchWordsDataStructureBenchmarksTests
{
    private const int SmallestWordCount = 200;

    // Setup reads every second pattern off a word it added, so those searches match; none is searched
    // twice, so the count can never exceed the pattern count.
    private const int MinimumMatchCount = SmallestWordCount / 2;
    private const int MaximumMatchCount = SmallestWordCount;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload()
    {
        Assert.InRange(BuildHarness().LinearScan(), MinimumMatchCount, MaximumMatchCount);
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());
    }

    [Fact]
    public void LinearScan_TwoHundredSeededWords_AgreesWithTrie()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Trie(), harness.LinearScan());
    }

    [Fact]
    public void Trie_TwoHundredSeededWords_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.Trie());
    }

    private static DesignAddAndSearchWordsDataStructureBenchmarks BuildHarness()
    {
        var harness = new DesignAddAndSearchWordsDataStructureBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
