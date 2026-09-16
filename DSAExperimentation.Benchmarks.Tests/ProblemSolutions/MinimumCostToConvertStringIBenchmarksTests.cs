using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumCostToConvertStringIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a per-position lookup into a raw 26x26 distance matrix
// against the same lookup into a built LetterNetwork - so a harness whose arms disagree is timing two
// different conversion graphs. Both arms answer with a long total cost, which they compare directly.
// The rules and the strings are drawn from two seeded streams, so the same StringLength must rebuild
// the same conversion.
public sealed partial class MinimumCostToConvertStringIBenchmarksTests
{
    private const int SmallestStringLength = 100;

    [Fact]
    public void Setup_SameStringLength_RebuildsTheSameConversion() =>
        Assert.Equal(BuildHarness().BruteForceFloydWarshall(), BuildHarness().BruteForceFloydWarshall());

    [Fact]
    public void BruteForceFloydWarshall_SeededConversionRing_AgreesWithAllPairsShortestPaths()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.AllPairsShortestPaths(), harness.BruteForceFloydWarshall());
    }

    [Fact]
    public void AllPairsShortestPaths_SeededConversionRing_AgreesWithBruteForceFloydWarshall()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceFloydWarshall(), harness.AllPairsShortestPaths());
    }

    private static MinimumCostToConvertStringIBenchmarks BuildHarness()
    {
        var harness = new MinimumCostToConvertStringIBenchmarks { StringLength = SmallestStringLength };
        harness.Setup();

        return harness;
    }
}
