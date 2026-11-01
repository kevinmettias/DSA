using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistributeCandiesAmongChildrenIIBenchmarks (ARCHITECTURE 17.9): an expected
// count known without either arm. The class carries no [GlobalSetup]: it derives candyCount from
// Limit as four times that limit, and three children capped at Limit can hold at most three times
// it, so at both parameters the candy count is larger than any legal distribution can place and
// the answer is zero.
public sealed partial class DistributeCandiesAmongChildrenIIBenchmarksTests
{
    private const int SmallestLimit = 200;
    private const long ExpectedWaysWhenTheCandyCountExceedsThreeLimits = 0;

    [Fact]
    public void BruteForce_FourLimitsOfCandyUnderAThreeLimitCeiling_PlacesNothing() =>
        Assert.Equal(ExpectedWaysWhenTheCandyCountExceedsThreeLimits, BuildHarness().BruteForce());

    private static DistributeCandiesAmongChildrenIIBenchmarks BuildHarness() =>
        new() { Limit = SmallestLimit };
}
