using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumScoreWithCoPrimeElementBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the fresh O(n) gcd scan per candidate against the two
// precomputed divisor sieves - so a harness whose arms disagree is timing two different problems.
// Setup draws the values from one fixed seed, so the same Length must rebuild the same workload;
// otherwise two published numbers were never comparable in the first place.
public sealed partial class MaximumScoreWithCoPrimeElementBenchmarksTests
{
    private const int SmallestLength = 100;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SeededValueRun_AgreesWithDivisorSieve()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DivisorSieve(), harness.BruteForce());
    }

    [Fact]
    public void DivisorSieve_SeededValueRun_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.DivisorSieve());
    }

    private static MaximumScoreWithCoPrimeElementBenchmarks BuildHarness()
    {
        var harness = new MaximumScoreWithCoPrimeElementBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
