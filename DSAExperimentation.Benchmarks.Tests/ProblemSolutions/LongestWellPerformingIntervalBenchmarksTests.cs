using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestWellPerformingIntervalBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - rescanning every candidate interval against the
// prefix-score map that answers "has this score been seen before" in one pass - so a harness whose
// arms disagree is timing two different problems. Both arms return a bare interval length. Setup
// draws the hours from one fixed seed, so the same Length must rebuild the same hour array and
// therefore the same interval length.
public sealed partial class LongestWellPerformingIntervalBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());
        Assert.Equal(BuildHarness().HashMapPrefixScore(), BuildHarness().HashMapPrefixScore());
    }

    [Fact]
    public void BruteForce_CoinFlipHours_AgreesWithHashMapPrefixScore()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapPrefixScore(), harness.BruteForce());
    }

    [Fact]
    public void HashMapPrefixScore_CoinFlipHours_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.HashMapPrefixScore());
    }

    private static LongestWellPerformingIntervalBenchmarks BuildHarness()
    {
        var harness = new LongestWellPerformingIntervalBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
