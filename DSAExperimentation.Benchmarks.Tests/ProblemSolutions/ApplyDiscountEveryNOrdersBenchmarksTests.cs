using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ApplyDiscountEveryNOrdersBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot
// pin. Each arm is handed its own cashier in [GlobalSetup], and a cashier counts its customers, so every test's
// second call on one harness bills a later customer - one the generic check, which calls each arm once on a fresh
// harness, never reaches. Both walk the same bill, which requests every product id in reverse catalogue order, so
// a positive total is also asserted: a zero bill would mean the walk found no product at all. Bills are doubles,
// so the comparison carries the class's own named tolerance rather than exact equality.
public sealed partial class ApplyDiscountEveryNOrdersBenchmarksTests
{
    // The smaller of Setup's [Params(20, 200)] catalogue sizes.
    private const int SmallestProductCount = 20;

    // Both arms sum the same per-product charges, so agreement is expected far below this; the
    // tolerance exists so a last-bit difference in the two accumulations cannot fail the harness
    // for the wrong reason.
    private const double RelativeTolerance = 1E-09;

    [Fact]
    public void LinearScanLookup_TwentyProductBill_AgreesWithHashMapLookup()
    {
        var harness = BuildHarness();

        Assert.True(harness.LinearScanLookup() > 0);
        Assert.Equal(harness.HashMapLookup(), harness.LinearScanLookup(), RelativeTolerance);
    }

    [Fact]
    public void HashMapLookup_TwentyProductBill_AgreesWithLinearScanLookup()
    {
        var harness = BuildHarness();

        Assert.True(harness.HashMapLookup() > 0);
        Assert.Equal(harness.LinearScanLookup(), harness.HashMapLookup(), RelativeTolerance);
    }

    private static ApplyDiscountEveryNOrdersBenchmarks BuildHarness()
    {
        var harness = new ApplyDiscountEveryNOrdersBenchmarks { ProductCount = SmallestProductCount };
        harness.Setup();

        return harness;
    }
}
