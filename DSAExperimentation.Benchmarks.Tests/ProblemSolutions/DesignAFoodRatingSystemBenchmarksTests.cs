using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignAFoodRatingSystemBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot
// pin: a bound on every reported name that follows from the workload's construction rather than from either arm.
// Setup builds the constructor's foods/cuisines/ratings, one superseding rating per food and the query cuisines,
// from one fixed seed, and each arm returns the name HighestRated reported for every query, in order. Setup names
// its foods "food" followed by LowercaseNames.Of(i), so every reported name is that prefix and then letters only.
public sealed partial class DesignAFoodRatingSystemBenchmarksTests
{
    private const int SmallestCount = 200;

    // The documented shape of a reported name: the prefix, then the two letters at most that name
    // two hundred foods.
    private const string FoodPrefix = "food";
    private const int MaxNameLettersAfterPrefix = 2;

    [Fact]
    public void LinearScan_TwoHundredSeededFoods_ReportsOnlyFoodNames() =>
        AssertReportsOnlyFoodNames(BuildHarness().LinearScan());

    [Fact]
    public void LazyDeletionHeap_TwoHundredSeededFoods_ReportsOnlyFoodNames() =>
        AssertReportsOnlyFoodNames(BuildHarness().LazyDeletionHeap());

    private static void AssertReportsOnlyFoodNames(string[] reported) =>
        Assert.All(reported, AssertIsFoodName);

    private static void AssertIsFoodName(string name)
    {
        Assert.StartsWith(FoodPrefix, name);

        var letters = name[FoodPrefix.Length..];

        Assert.InRange(letters.Length, 1, MaxNameLettersAfterPrefix);
        Assert.All(letters, letter => Assert.InRange(letter, 'a', 'z'));
    }

    private static DesignAFoodRatingSystemBenchmarks BuildHarness()
    {
        var harness = new DesignAFoodRatingSystemBenchmarks { Count = SmallestCount };
        harness.Setup();

        return harness;
    }
}
