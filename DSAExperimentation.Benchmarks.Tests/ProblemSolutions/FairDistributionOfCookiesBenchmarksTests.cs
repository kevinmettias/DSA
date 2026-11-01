using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FairDistributionOfCookiesBenchmarks (ARCHITECTURE 17.9): a bound the
// workload's shape fixes without either arm. The answer is the minimised maximum load, a plain int.
// Setup draws every bag from [1, MaxBagSize), so every bag is positive and the whole workload's
// cookie count is below BagCount * MaxBagSize; the minimised maximum therefore has to be at least
// one cookie and can never exceed the entire workload.
public sealed partial class FairDistributionOfCookiesBenchmarksTests
{
    private const int SmallestBagCount = 6;
    private const int MaxBagSize = 20;

    private const int MinUnfairness = 1;

    // No child can end up holding more than every cookie there is - a single child taking the whole
    // workload is always an available assignment, so the minimised maximum is bounded by the total.
    private const int MaxUnfairness = (SmallestBagCount * MaxBagSize) - 1;

    [Fact]
    public void RecursiveBacktracking_SeededBagsAndThreeChildren_StaysBetweenOneCookieAndTheWholeWorkload() =>
        Assert.InRange(BuildHarness().RecursiveBacktracking(), MinUnfairness, MaxUnfairness);

    private static FairDistributionOfCookiesBenchmarks BuildHarness()
    {
        var harness = new FairDistributionOfCookiesBenchmarks { BagCount = SmallestBagCount };
        harness.Setup();

        return harness;
    }
}
