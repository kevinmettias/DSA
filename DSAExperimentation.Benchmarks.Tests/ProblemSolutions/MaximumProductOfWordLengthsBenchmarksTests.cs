using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumProductOfWordLengthsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the bitmask-and-longest-word map against the
// per-character pair scan - so a harness whose arms disagree is timing two different problems.
// Setup draws the words from one fixed seed, so the same WordCount must rebuild the same workload;
// otherwise two published numbers were never comparable in the first place.
public sealed partial class MaximumProductOfWordLengthsBenchmarksTests
{
    private const int SmallestWordCount = 100;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BitmaskHashMap(), BuildHarness().BitmaskHashMap());

    [Fact]
    public void BitmaskHashMap_DisjointAlphabetHalves_AgreesWithCharacterScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CharacterScan(), harness.BitmaskHashMap());
    }

    [Fact]
    public void CharacterScan_DisjointAlphabetHalves_AgreesWithBitmaskHashMap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BitmaskHashMap(), harness.CharacterScan());
    }

    private static MaximumProductOfWordLengthsBenchmarks BuildHarness()
    {
        var harness = new MaximumProductOfWordLengthsBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
