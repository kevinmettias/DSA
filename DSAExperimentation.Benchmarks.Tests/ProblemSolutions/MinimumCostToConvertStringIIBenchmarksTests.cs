using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumCostToConvertStringIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a DP walk over a raw distance matrix keyed by substring
// index against the same walk over a built SubstringNetwork - so a harness whose arms disagree is
// timing two different conversion graphs. Both arms answer with a long total cost, which they compare
// directly. The rules and the strings are drawn from two seeded streams, so the same StringLength must
// rebuild the same conversion.
public sealed partial class MinimumCostToConvertStringIIBenchmarksTests
{
    private const int SmallestStringLength = 100;

    [Fact]
    public void Setup_SameStringLength_RebuildsTheSameConversion() =>
        Assert.Equal(BuildHarness().BruteForceFloydWarshall(), BuildHarness().BruteForceFloydWarshall());

    [Fact]
    public void BruteForceFloydWarshall_SeededSubstringRules_AgreesWithAllPairsShortestPaths()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.AllPairsShortestPaths(), harness.BruteForceFloydWarshall());
    }

    [Fact]
    public void AllPairsShortestPaths_SeededSubstringRules_AgreesWithBruteForceFloydWarshall()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceFloydWarshall(), harness.AllPairsShortestPaths());
    }

    private static MinimumCostToConvertStringIIBenchmarks BuildHarness()
    {
        var harness = new MinimumCostToConvertStringIIBenchmarks { StringLength = SmallestStringLength };
        harness.Setup();

        return harness;
    }
}
