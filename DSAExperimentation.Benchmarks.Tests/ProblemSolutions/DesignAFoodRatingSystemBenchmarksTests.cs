using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignAFoodRatingSystemBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot
// pin: a bound on every reported name that follows from the workload's construction rather than from either arm.
// Setup builds the constructor's foods/cuisines/ratings, one superseding rating per food and the query cuisines,
// from one fixed seed, and each arm returns the name HighestRated reported for every query, in order. Setup names
// its foods "food{i}", so no reported name is longer than that over a Count below four digits.
public sealed partial class DesignAFoodRatingSystemBenchmarksTests
{
    private const int SmallestCount = 200;

    // The documented shape of a reported name: "food{i}" over a Count below four digits.
    private const int MaxReportedNameLength = 8;

    // A cuisine with nothing rated reports no name at all.
    private const int MinimumReportedNameLength = 0;

    [Fact]
    public void LinearScan_TwoHundredSeededFoods_ReportsOnlyFoodNames() =>
        AssertReportsOnlyFoodNames(BuildHarness().LinearScan());

    [Fact]
    public void LazyDeletionHeap_TwoHundredSeededFoods_ReportsOnlyFoodNames() =>
        AssertReportsOnlyFoodNames(BuildHarness().LazyDeletionHeap());

    private static void AssertReportsOnlyFoodNames(string[] reported) =>
        Assert.All(reported, name => Assert.InRange(name.Length, MinimumReportedNameLength, MaxReportedNameLength));

    private static DesignAFoodRatingSystemBenchmarks BuildHarness()
    {
        var harness = new DesignAFoodRatingSystemBenchmarks { Count = SmallestCount };
        harness.Setup();

        return harness;
    }
}
