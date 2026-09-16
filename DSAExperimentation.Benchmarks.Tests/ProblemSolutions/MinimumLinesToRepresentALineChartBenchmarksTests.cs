using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumLinesToRepresentALineChartBenchmarks (ARCHITECTURE 17.9): both
// arms are MinimumLinesToRepresentALineChartSolution's, the same methods
// MinimumLinesToRepresentALineChartTests proves correct, and both return the fewest lines that
// cover the (day, price) points. The two arms differ only in how the ordering is established -
// BCL Array.Sort plus a floating-point slope against this repo's own MergeSort plus an exact
// cross-product test - so their answers must be identical, not merely close.
public sealed partial class MinimumLinesToRepresentALineChartBenchmarksTests
{
    // The smallest declared [Params] value: Setup builds one distinct day per point, so a
    // shorter point set is the cheaper way to reach the same ordering comparison.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().ArraySortFloatingSlope(),
            BuildHarness().ArraySortFloatingSlope());

    [Fact]
    public void ArraySortFloatingSlope_AgreesWithMergeSortIntegerSlope()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortIntegerSlope(), harness.ArraySortFloatingSlope());
    }

    [Fact]
    public void MergeSortIntegerSlope_AgreesWithArraySortFloatingSlope()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArraySortFloatingSlope(), harness.MergeSortIntegerSlope());
    }

    private static MinimumLinesToRepresentALineChartBenchmarks BuildHarness()
    {
        var harness = new MinimumLinesToRepresentALineChartBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
