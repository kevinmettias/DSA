using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PalindromePairsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same pair set - the concatenate-and-check brute force against the reversed
// complement HashMap lookup - so a harness whose arms disagree is timing two different problems.
// Setup draws the word list from one seeded Random into a set, so the same WordCount must rebuild the
// same words; the smallest tuned WordCount keeps the quadratic arm affordable.
//
// WEAK BY CONSTRUCTION, and reported as such: both arms return only the pair count, not the pairs, so
// agreement witnesses that the two strategies found the same number of index pairs - two strategies
// that returned different pairs of equal size would still agree.
public sealed partial class PalindromePairsBenchmarksTests
{
    private const int SmallestWordCount = 80;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SmallestWordCount_AgreesWithHashMapComplementLookup()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapComplementLookup(), harness.BruteForce());
    }

    [Fact]
    public void HashMapComplementLookup_SmallestWordCount_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.HashMapComplementLookup());
    }

    private static PalindromePairsBenchmarks BuildHarness()
    {
        var harness = new PalindromePairsBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
