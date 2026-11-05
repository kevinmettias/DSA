using DSAExperimentation.LeetCode.Solutions.MinimumLinesToRepresentALineChart;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumLinesToRepresentALineChart;

// Harness only. Both strategies are MinimumLinesToRepresentALineChartSolution's - a BCL
// Array.Sort with floating-point slope comparison, and this repo's own MergeSort over
// ArrayIndexedSequence with the exact cross-product collinearity test - so each is
// asserted here under its own name. The pre-migration test only covered the second one;
// the floating-slope baseline (previously untested scaffolding inlined in the benchmark)
// is asserted here for the first time, against the same cases.
public sealed partial class MinimumLinesToRepresentALineChartSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            // A single point draws no line at all.
            { [[5, 5]], 0 },

            // Two points are always exactly one line, whatever their slope.
            { [[1, 1], [2, 3]], 1 },

            // Three collinear points still need only one line.
            { [[1, 1], [2, 2], [3, 3]], 1 },

            // Unsorted input whose sorted order genuinely bends.
            { [[1, 1], [3, 3], [2, 5]], 2 },

            // Every consecutive triple bends the other way.
            { [[1, 1], [2, 2], [3, 1], [4, 2]], 3 },

            // Collinear at the problem's own coordinate bounds: the cross-product test
            // multiplies values near 1e9, which overflows int and only stays correct
            // because the operands are widened to long first.
            { [[1, 1], [500_000_000, 500_000_000], [1_000_000_000, 1_000_000_000]], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumLinesByArraySortFloatingSlope_Cases_CountsSlopeChanges(
        int[][] stockPrices, int expected) =>
        Assert.Equal(
            expected,
            MinimumLinesToRepresentALineChartSolution.MinimumLinesByArraySortFloatingSlope(stockPrices));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumLinesByMergeSortIntegerSlope_Cases_CountsSlopeChanges(
        int[][] stockPrices, int expected) =>
        Assert.Equal(
            expected,
            MinimumLinesToRepresentALineChartSolution.MinimumLinesByMergeSortIntegerSlope(stockPrices));
}
