using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistributeCandiesAmongChildrenIBenchmarks (ARCHITECTURE 17.9): an expected
// count known without either arm. The class carries no [GlobalSetup]: both arms are handed the same
// CandyCount as the limit as well, and at 10 candies that limit is higher than the total, so it
// never binds and the answer is the unbounded stars-and-bars count C(10 + 2, 2) = 66.
public sealed partial class DistributeCandiesAmongChildrenIBenchmarksTests
{
    private const int SmallestCandyCount = 10;
    private const int ExpectedWaysForTenCandiesUnderATenCandyLimit = 66;

    [Fact]
    public void BruteForce_TenCandiesUnderATenCandyLimit_CountsTheStarsAndBarsWays() =>
        Assert.Equal(ExpectedWaysForTenCandiesUnderATenCandyLimit, BuildHarness().BruteForce());

    private static DistributeCandiesAmongChildrenIBenchmarks BuildHarness() =>
        new() { CandyCount = SmallestCandyCount };
}
