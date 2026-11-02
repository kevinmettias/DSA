using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistributeRepeatingIntegersBenchmarks (ARCHITECTURE 17.9): an expected
// verdict known from the class comment's own guarantee - the stock buckets were filled with the
// shared sum of the orders, so a valid distribution always exists and the answer is true.
public sealed partial class DistributeRepeatingIntegersBenchmarksTests
{
    private const int SmallestValueCount = 3;
    private const bool ExpectedDistributabilityForTheSharedStockWorkload = true;

    [Fact]
    public void CanDistributeByNaiveBacktracking_SharedStockBuckets_PlacesEveryOrder() =>
        Assert.Equal(ExpectedDistributabilityForTheSharedStockWorkload, BuildHarness().CanDistributeByNaiveBacktracking());

    private static DistributeRepeatingIntegersBenchmarks BuildHarness()
    {
        var harness = new DistributeRepeatingIntegersBenchmarks { ValueCount = SmallestValueCount };
        harness.Setup();

        return harness;
    }
}
