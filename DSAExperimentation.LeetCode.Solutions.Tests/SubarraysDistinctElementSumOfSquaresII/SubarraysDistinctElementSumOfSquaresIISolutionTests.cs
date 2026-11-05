using DSAExperimentation.LeetCode.Solutions.SubarraysDistinctElementSumOfSquaresII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SubarraysDistinctElementSumOfSquaresII;

// Harness only: the algorithms live in SubarraysDistinctElementSumOfSquaresIISolution.
// One test method per strategy over one shared set of cases, so a failure names the
// strategy that broke. The one case is an array of distinct values, where every
// subarray's distinct count is its length. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class SubarraysDistinctElementSumOfSquaresIISolutionTests
{
    public static TheoryData<int[], long> Cases =>
        new()
        {
            { [1, 2, 3], 20 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SumOfSquaresByBruteForce_Cases_ReturnsModuloSumOfSquares(int[] nums, long expected)
    {
        var actual = SubarraysDistinctElementSumOfSquaresIISolution.SumOfSquaresByBruteForce(nums);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SumOfSquaresByRangeFenwickTree_Cases_ReturnsModuloSumOfSquares(int[] nums, long expected)
    {
        var actual = SubarraysDistinctElementSumOfSquaresIISolution.SumOfSquaresByRangeFenwickTree(nums);

        Assert.Equal(expected, actual);
    }
}
