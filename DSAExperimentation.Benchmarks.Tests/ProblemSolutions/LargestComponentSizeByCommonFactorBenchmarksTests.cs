using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LargestComponentSizeByCommonFactorBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - an O(n^2) pairwise gcd sweep against per-factor
// union - so a harness whose arms disagree is timing two different problems. Every value is a product
// of the same small prime pool, so real overlaps occur and each arm reports a genuinely merged
// component rather than the Length singletons a coprime draw would give. Setup's workload is seeded,
// so the same Length must rebuild it; otherwise two published numbers were never comparable.
public sealed partial class LargestComponentSizeByCommonFactorBenchmarksTests
{
    private const int SmallestLength = 50;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().PairwiseGcdScan(), BuildHarness().PairwiseGcdScan());

    [Fact]
    public void PairwiseGcdScan_SharedPrimePool_AgreesWithDisjointSetByPrimeFactor()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetByPrimeFactor(), harness.PairwiseGcdScan());
    }

    [Fact]
    public void DisjointSetByPrimeFactor_SharedPrimePool_AgreesWithPairwiseGcdScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwiseGcdScan(), harness.DisjointSetByPrimeFactor());
    }

    private static LargestComponentSizeByCommonFactorBenchmarks BuildHarness()
    {
        var harness = new LargestComponentSizeByCommonFactorBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
