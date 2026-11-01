using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistributeRepeatingIntegersBenchmarks (ARCHITECTURE 17.9): an expected
// verdict known from the class comment's own guarantee - the stock buckets were filled with the
// shared sum of the orders, so a valid distribution always exists and the answer is true. The arms
// copy the prepared stock before searching it, so one harness is safe to read twice, as
// BenchmarkDotNet reads it across iterations; the second assertion holds a second read to a fresh
// build's.
public sealed partial class DistributeRepeatingIntegersBenchmarksTests
{
    private const int SmallestValueCount = 3;
    private const bool ExpectedDistributabilityForTheSharedStockWorkload = true;

    [Fact]
    public void Setup_SharedStockBuckets_PlaceEveryOrderAndRebuildTheSameWorkload()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedDistributabilityForTheSharedStockWorkload, harness.CanDistributeByNaiveBacktracking());
        Assert.Equal(harness.CanDistributeByNaiveBacktracking(), BuildHarness().CanDistributeByNaiveBacktracking());
    }

    private static DistributeRepeatingIntegersBenchmarks BuildHarness()
    {
        var harness = new DistributeRepeatingIntegersBenchmarks { ValueCount = SmallestValueCount };
        harness.Setup();

        return harness;
    }
}
