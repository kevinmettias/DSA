using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestWordInDictionaryBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - testing each word's every prefix against a
// dictionary set against walking a trie of the words themselves - so a harness whose arms disagree
// is timing two different problems. Both arms return the longest buildable word, and LC 720 pins it
// (the longest, then lexicographically smallest, tie broken by the search itself), so the two
// strings are compared directly. Setup grows each word from the previous one by a seeded letter
// draw, so the same WordCount must rebuild the same word list.
public sealed partial class LongestWordInDictionaryBenchmarksTests
{
    private const int SmallestWordCount = 50;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().DictionaryScanPerPrefix(), BuildHarness().DictionaryScanPerPrefix());
        Assert.Equal(BuildHarness().LowercaseTrieWalk(), BuildHarness().LowercaseTrieWalk());
    }

    [Fact]
    public void DictionaryScanPerPrefix_GrownPrefixChains_AgreesWithLowercaseTrieWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LowercaseTrieWalk(), harness.DictionaryScanPerPrefix());
    }

    [Fact]
    public void LowercaseTrieWalk_GrownPrefixChains_AgreesWithDictionaryScanPerPrefix()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DictionaryScanPerPrefix(), harness.LowercaseTrieWalk());
    }

    private static LongestWordInDictionaryBenchmarks BuildHarness()
    {
        var harness = new LongestWordInDictionaryBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
