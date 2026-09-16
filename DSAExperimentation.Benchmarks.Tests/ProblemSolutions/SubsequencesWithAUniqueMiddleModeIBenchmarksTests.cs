using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SubsequencesWithAUniqueMiddleModeIBenchmarks (ARCHITECTURE 17.9): both
// arms answer the same question - how many LC 3395 subsequences have a unique middle mode - one
// by enumerating them, one by modular combinatorics over distinct pairs, so a harness whose arms
// disagree is timing two different problems. Setup's values are seeded from a small range, so
// the same length must rebuild the same workload.
public sealed partial class SubsequencesWithAUniqueMiddleModeIBenchmarksTests
{
    private const int SmallestLength = 10;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_AgreesWithModularCombinatorics()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.ModularCombinatorics());
    }

    [Fact]
    public void ModularCombinatorics_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ModularCombinatorics(), harness.BruteForce());
    }

    private static SubsequencesWithAUniqueMiddleModeIBenchmarks BuildHarness()
    {
        var harness = new SubsequencesWithAUniqueMiddleModeIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
