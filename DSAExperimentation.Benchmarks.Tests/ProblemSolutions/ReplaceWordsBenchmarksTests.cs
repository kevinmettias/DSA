using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReplaceWordsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - scanning the whole dictionary for each sentence word against
// walking this repo's LowercaseTrie for the shortest root - so a harness whose arms disagree is
// timing two different problems. Setup draws both the dictionary and the sentence from one fixed
// seed, so the same DictionarySize must rebuild the same pair; otherwise two published numbers were
// never comparable in the first place.
//
// Both arms read the hoisted dictionary and sentence without writing to either, so one harness is
// safe to call twice in either order and the single-harness rule holds.
public sealed partial class ReplaceWordsBenchmarksTests
{
    private const int SmallestDictionarySize = 50;

    [Fact]
    public void Setup_SameDictionarySize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().DictionaryScanPerWord(),
            BuildHarness().DictionaryScanPerWord());

    [Fact]
    public void DictionaryScanPerWord_PrefixedAndRandomWords_AgreesWithLowercaseTrieWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LowercaseTrieWalk(), harness.DictionaryScanPerWord());
    }

    [Fact]
    public void LowercaseTrieWalk_PrefixedAndRandomWords_AgreesWithDictionaryScanPerWord()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DictionaryScanPerWord(), harness.LowercaseTrieWalk());
    }

    private static ReplaceWordsBenchmarks BuildHarness()
    {
        var harness = new ReplaceWordsBenchmarks { DictionarySize = SmallestDictionarySize };
        harness.Setup();

        return harness;
    }
}
