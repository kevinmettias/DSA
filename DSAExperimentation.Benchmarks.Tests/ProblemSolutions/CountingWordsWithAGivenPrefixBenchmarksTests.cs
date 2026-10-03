using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountingWordsWithAGivenPrefixBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - scanning each word's own prefix against inserting
// every word into a trie and asking it - so a harness whose arms disagree is timing two different
// problems, not two ways of answering one. Setup draws the words from one fixed seed, so the same
// WordCount must rebuild the same words; otherwise two published numbers were never comparable in the
// first place.
//
// The word array is private and the match count is the only thing either arm reports, so the
// documented shape is asserted through that: the answer counts words of a WordCount-long array, and
// with a single-letter prefix over a 26-letter alphabet the seeded run at the largest WordCount has a
// non-trivial, non-zero share of them. At the smallest, ten words, the share is expected to be zero
// or one, and this seed happens to give zero, so the non-zero check is made at the largest size.
public sealed partial class CountingWordsWithAGivenPrefixBenchmarksTests
{
    private const int SmallestWordCount = 10;
    private const int LargestWordCount = 100;

    // A single-letter prefix over a 26-letter alphabet leaves the seeded run with a non-trivial
    // share of matches - the arm counts words, not flags, so a zero would mean the alphabet never
    // put the prefix at the front of any word.
    private const int FewestPrefixMatches = 1;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWords() =>
        Assert.Equal(BuildHarness().StartsWithScan(), BuildHarness().StartsWithScan());

    [Fact]
    public void StartsWithScan_LargestWordCount_FindsSomeWordsWithThePrefix() =>
        Assert.InRange(BuildHarness(LargestWordCount).StartsWithScan(), FewestPrefixMatches, LargestWordCount);

    [Fact]
    public void StartsWithScan_SingleLetterPrefix_AgreesWithTriePerWordHasPrefix()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TriePerWordHasPrefix(), harness.StartsWithScan());
    }

    [Fact]
    public void TriePerWordHasPrefix_SingleLetterPrefix_AgreesWithStartsWithScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StartsWithScan(), harness.TriePerWordHasPrefix());
    }

    private static CountingWordsWithAGivenPrefixBenchmarks BuildHarness(int wordCount = SmallestWordCount)
    {
        var harness = new CountingWordsWithAGivenPrefixBenchmarks { WordCount = wordCount };
        harness.Setup();

        return harness;
    }
}
