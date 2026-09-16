using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ApplyDiscountEveryNOrdersBenchmarks (ARCHITECTURE 17.9): its two arms are
// ApplyDiscountEveryNOrdersSolution's competing strategies for the same question - a linear catalogue scan against
// a hash-map lookup - so a harness whose arms disagree is pricing two different catalogues. Each arm is handed its
// own cashier in [GlobalSetup] and both walk the same bill, which requests every product id in reverse catalogue
// order, so a positive total is also asserted: a zero bill would mean the walk found no product at all. Bills are
// doubles, so the comparison carries the class's own named tolerance rather than exact equality.
public sealed partial class ApplyDiscountEveryNOrdersBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] catalogue sizes.
    private const int SmallestProductCount = 200;

    // Both arms sum the same per-product charges, so agreement is expected far below this; the
    // tolerance exists so a last-bit difference in the two accumulations cannot fail the harness
    // for the wrong reason.
    private const double RelativeTolerance = 1E-09;

    [Fact]
    public void Setup_SameProductCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().LinearScanLookup(),
            BuildHarness().LinearScanLookup(),
            RelativeTolerance);

    [Fact]
    public void LinearScanLookup_TwoHundredProductBill_AgreesWithHashMapLookup()
    {
        var harness = BuildHarness();

        Assert.True(harness.LinearScanLookup() > 0);
        Assert.Equal(harness.HashMapLookup(), harness.LinearScanLookup(), RelativeTolerance);
    }

    [Fact]
    public void HashMapLookup_TwoHundredProductBill_AgreesWithLinearScanLookup()
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
