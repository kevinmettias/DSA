using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FinalPricesWithASpecialDiscountInAShopBenchmarks (ARCHITECTURE 17.9): the
// generic arms check only compares the arms with each other, so this holds the baseline's answer to
// the workload's own shape - one final price per item Setup drew.
public sealed partial class FinalPricesWithASpecialDiscountInAShopBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void BruteForce_SeededPrices_ReturnsOneFinalPricePerItem() =>
        Assert.Equal(SmallestLength, BuildHarness().BruteForce().Length);

    private static FinalPricesWithASpecialDiscountInAShopBenchmarks BuildHarness()
    {
        var harness = new FinalPricesWithASpecialDiscountInAShopBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
