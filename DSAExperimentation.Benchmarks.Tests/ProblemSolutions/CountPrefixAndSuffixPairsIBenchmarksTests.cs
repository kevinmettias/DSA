using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountPrefixAndSuffixPairsIBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on a rebuilt workload, so this asserts only the shape of the
// words Setup draws through PrefixSuffixPairWorkloads, which no arm's answer reveals.
public sealed partial class CountPrefixAndSuffixPairsIBenchmarksTests
{
    private const int SmallestWordCount = 25;

    // Mirrors the benchmark's own seed and word-length bound, so the workload asserted here is the
    // one Setup builds.
    private const int Seed = 3042;

    private const int MaxWordLength = 10;

    [Fact]
    public void Setup_SmallestWordCount_DrawsWordsWithinTheLengthBound()
    {
        var words = PrefixSuffixPairWorkloads.BuildWords(SmallestWordCount, MaxWordLength, Seed);

        // The documented shape: the fixture draws each word's length from [1, maxLength] over a
        // four-letter alphabet, so every word is a real candidate for a prefix/suffix span.
        Assert.Equal(SmallestWordCount, words.Length);
        Assert.All(words, word => Assert.InRange(word.Length, 1, MaxWordLength));
    }
}
