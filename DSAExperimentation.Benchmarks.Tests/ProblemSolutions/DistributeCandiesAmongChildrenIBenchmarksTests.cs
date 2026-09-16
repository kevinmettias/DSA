using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistributeCandiesAmongChildrenIBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the textbook double loop against the
// stars-and-bars/inclusion-exclusion closed form - so a harness whose arms disagree is timing two
// different problems. The class carries no [GlobalSetup]: both arms are handed the same CandyCount
// as the limit as well, and at 10 candies that limit is higher than the total, so it never binds
// and the answer is the unbounded stars-and-bars count C(10 + 2, 2) = 66.
public sealed partial class DistributeCandiesAmongChildrenIBenchmarksTests
{
    private const int SmallestCandyCount = 10;
    private const int ExpectedWaysForTenCandiesUnderATenCandyLimit = 66;

    [Fact]
    public void BruteForce_TenCandiesUnderATenCandyLimit_CountsTheStarsAndBarsWaysAndAgreesWithInclusionExclusion()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedWaysForTenCandiesUnderATenCandyLimit, harness.BruteForce());
        Assert.Equal(harness.InclusionExclusion(), harness.BruteForce());
    }

    [Fact]
    public void InclusionExclusion_TenCandiesUnderATenCandyLimit_CountsTheStarsAndBarsWaysAndAgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedWaysForTenCandiesUnderATenCandyLimit, harness.InclusionExclusion());
        Assert.Equal(harness.BruteForce(), harness.InclusionExclusion());
    }

    private static DistributeCandiesAmongChildrenIBenchmarks BuildHarness() =>
        new() { CandyCount = SmallestCandyCount };
}
