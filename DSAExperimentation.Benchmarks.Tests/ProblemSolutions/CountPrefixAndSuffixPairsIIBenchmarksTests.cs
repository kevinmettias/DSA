using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountPrefixAndSuffixPairsIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - comparing each candidate pair directly against
// walking a LowercaseTrie - so a harness whose arms disagree is timing two different problems. Setup
// draws words from a fixed seed through PrefixSuffixPairWorkloads, so the same WordCount must
// rebuild the same words.
public sealed partial class CountPrefixAndSuffixPairsIIBenchmarksTests
{
    private const int SmallestWordCount = 100;

    // Mirrors the benchmark's own seed and word-length bound, so the workload asserted here is the
    // one Setup builds.
    private const int Seed = 3045;

    private const int MaxWordLength = 30;

    [Fact]
    public void Setup_SmallestWordCount_RebuildsTheSameWorkload()
    {
        var words = PrefixSuffixPairWorkloads.BuildWords(SmallestWordCount, MaxWordLength, Seed);

        // The documented shape: the fixture draws each word's length from [1, maxLength] over a
        // four-letter alphabet, so the trie is built over words that genuinely share prefixes.
        Assert.Equal(SmallestWordCount, words.Length);
        Assert.All(words, word => Assert.InRange(word.Length, 1, MaxWordLength));
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());
    }

    [Fact]
    public void BruteForce_FourLetterAlphabet_AgreesWithLowercaseTrie()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LowercaseTrie(), harness.BruteForce());
    }

    [Fact]
    public void LowercaseTrie_FourLetterAlphabet_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.LowercaseTrie());
    }

    private static CountPrefixAndSuffixPairsIIBenchmarks BuildHarness()
    {
        var harness = new CountPrefixAndSuffixPairsIIBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
