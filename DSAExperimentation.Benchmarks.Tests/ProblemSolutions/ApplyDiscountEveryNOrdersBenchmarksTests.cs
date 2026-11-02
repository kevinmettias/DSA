using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ApplyDiscountEveryNOrdersBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot
// pin. Both arms walk the same bill, which requests every product id in reverse catalogue order, so a positive total
// is asserted on the baseline: a zero bill would mean the walk found no product at all.
public sealed partial class ApplyDiscountEveryNOrdersBenchmarksTests
{
    // The smaller of Setup's [Params(20, 200)] catalogue sizes.
    private const int SmallestProductCount = 20;

    [Fact]
    public void LinearScanLookup_TwentyProductBill_ChargesAPositiveTotal() =>
        Assert.True(BuildHarness().LinearScanLookup() > 0);

    private static ApplyDiscountEveryNOrdersBenchmarks BuildHarness()
    {
        var harness = new ApplyDiscountEveryNOrdersBenchmarks { ProductCount = SmallestProductCount };
        harness.Setup();

        return harness;
    }
}
