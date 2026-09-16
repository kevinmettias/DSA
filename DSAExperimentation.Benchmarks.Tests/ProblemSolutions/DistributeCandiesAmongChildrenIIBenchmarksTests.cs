using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistributeCandiesAmongChildrenIIBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the same double loop LC 2928 uses against the
// long-valued stars-and-bars/inclusion-exclusion identity - so a harness whose arms disagree is
// timing two different problems. The class carries no [GlobalSetup]: it derives candyCount from
// Limit as four times that limit, and three children capped at Limit can hold at most three times
// it, so at both parameters the candy count is larger than any legal distribution can place and
// every arm must report zero. Both arms are read from the same Limit, so the same Limit must report
// that same zero.
public sealed partial class DistributeCandiesAmongChildrenIIBenchmarksTests
{
    private const int SmallestLimit = 200;
    private const long ExpectedWaysWhenTheCandyCountExceedsThreeLimits = 0;

    [Fact]
    public void BruteForce_FourLimitsOfCandyUnderAThreeLimitCeiling_PlacesNothingAndAgreesWithInclusionExclusion()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedWaysWhenTheCandyCountExceedsThreeLimits, harness.BruteForce());
        Assert.Equal(harness.InclusionExclusion(), harness.BruteForce());
    }

    [Fact]
    public void InclusionExclusion_FourLimitsOfCandyUnderAThreeLimitCeiling_PlacesNothingAndAgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedWaysWhenTheCandyCountExceedsThreeLimits, harness.InclusionExclusion());
        Assert.Equal(harness.BruteForce(), harness.InclusionExclusion());
    }

    private static DistributeCandiesAmongChildrenIIBenchmarks BuildHarness() =>
        new() { Limit = SmallestLimit };
}
