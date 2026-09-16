using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountPrimesBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - testing every candidate by trial division against sieving
// every composite out - so a harness whose arms disagree is timing two different problems. This
// benchmark has no [GlobalSetup]: Limit is the whole workload, so there is nothing to rebuild.
public sealed partial class CountPrimesBenchmarksTests
{
    private const int SmallestLimit = 2_000;

    [Fact]
    public void TrialDivision_SmallestLimit_AgreesWithSieveOfEratosthenes()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SieveOfEratosthenes(), harness.TrialDivision());
    }

    [Fact]
    public void SieveOfEratosthenes_SmallestLimit_AgreesWithTrialDivision()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TrialDivision(), harness.SieveOfEratosthenes());
    }

    private static CountPrimesBenchmarks BuildHarness() => new() { Limit = SmallestLimit };
}
