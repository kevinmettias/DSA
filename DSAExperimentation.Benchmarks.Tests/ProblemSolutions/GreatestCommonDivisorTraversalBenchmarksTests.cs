using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GreatestCommonDivisorTraversalBenchmarks (ARCHITECTURE 17.9): both arms are
// GreatestCommonDivisorTraversalSolution's - the O(n^2) pairwise gcd sweep against the
// per-prime-factor union - so a harness whose arms disagree is timing two different problems.
// Values are drawn as products of one small shared prime pool, so real overlaps occur and neither
// arm gets to answer from a trivially disconnected set. Both arms answer with a bare bool, so
// agreement says the two strategies reached the same verdict on the same values and nothing
// stronger. Setup draws the values once off that seed, so the same Length must rebuild the same
// array.
public sealed partial class GreatestCommonDivisorTraversalBenchmarksTests
{
    private const int SmallestLength = 50;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().CanTraverseAllPairsByPairwiseGcd(),
            BuildHarness().CanTraverseAllPairsByPairwiseGcd());

    [Fact]
    public void CanTraverseAllPairsByPairwiseGcd_SharedPrimePool_AgreesWithPrimeFactorUnion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanTraverseAllPairsByPrimeFactorUnion(), harness.CanTraverseAllPairsByPairwiseGcd());
    }

    [Fact]
    public void CanTraverseAllPairsByPrimeFactorUnion_SharedPrimePool_AgreesWithPairwiseGcd()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanTraverseAllPairsByPairwiseGcd(), harness.CanTraverseAllPairsByPrimeFactorUnion());
    }

    private static GreatestCommonDivisorTraversalBenchmarks BuildHarness()
    {
        var harness = new GreatestCommonDivisorTraversalBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
