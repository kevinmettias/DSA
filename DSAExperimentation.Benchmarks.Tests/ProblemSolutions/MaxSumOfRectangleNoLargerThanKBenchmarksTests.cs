using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaxSumOfRectangleNoLargerThanKBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - scanning every row-pair window against the sorted-set
// ceiling lookup over the same prefix rows - so a harness whose arms disagree is timing two different
// problems. Both arms return the largest rectangle sum that stays at or below the class's own K.
// Setup draws the cells from one fixed seed, so the same Rows must rebuild the same matrix and the
// same sum.
public sealed partial class MaxSumOfRectangleNoLargerThanKBenchmarksTests
{
    private const int SmallestRows = 200;

    [Fact]
    public void Setup_SameRows_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().BruteForceWindowScan(), BuildHarness().BruteForceWindowScan());
        Assert.Equal(BuildHarness().BstCeilingScan(), BuildHarness().BstCeilingScan());
    }

    [Fact]
    public void BruteForceWindowScan_SeededSignedCells_AgreesWithBstCeilingScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BstCeilingScan(), harness.BruteForceWindowScan());
    }

    [Fact]
    public void BstCeilingScan_SeededSignedCells_AgreesWithBruteForceWindowScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceWindowScan(), harness.BstCeilingScan());
    }

    private static MaxSumOfRectangleNoLargerThanKBenchmarks BuildHarness()
    {
        var harness = new MaxSumOfRectangleNoLargerThanKBenchmarks { Rows = SmallestRows };
        harness.Setup();

        return harness;
    }
}
