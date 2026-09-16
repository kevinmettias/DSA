using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountPrefixAndSuffixPairsIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - comparing each candidate pair character by character
// against comparing pre-built RollingHash values - so a harness whose arms disagree is timing two
// different problems. Setup draws words from a fixed seed through PrefixSuffixPairWorkloads and
// builds their hashes, so the same WordCount must rebuild both.
public sealed partial class CountPrefixAndSuffixPairsIBenchmarksTests
{
    private const int SmallestWordCount = 25;

    // Mirrors the benchmark's own seed and word-length bound, so the workload asserted here is the
    // one Setup builds.
    private const int Seed = 3042;

    private const int MaxWordLength = 50;

    [Fact]
    public void Setup_SmallestWordCount_RebuildsTheSameWorkload()
    {
        var words = PrefixSuffixPairWorkloads.BuildWords(SmallestWordCount, MaxWordLength, Seed);

        // The documented shape: the fixture draws each word's length from [1, maxLength] over a
        // four-letter alphabet, so every word is a real candidate for a prefix/suffix span.
        Assert.Equal(SmallestWordCount, words.Length);
        Assert.All(words, word => Assert.InRange(word.Length, 1, MaxWordLength));
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());
    }

    [Fact]
    public void BruteForce_FourLetterAlphabet_AgreesWithRollingHash()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RollingHash(), harness.BruteForce());
    }

    [Fact]
    public void RollingHash_FourLetterAlphabet_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.RollingHash());
    }

    private static CountPrefixAndSuffixPairsIBenchmarks BuildHarness()
    {
        var harness = new CountPrefixAndSuffixPairsIBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
