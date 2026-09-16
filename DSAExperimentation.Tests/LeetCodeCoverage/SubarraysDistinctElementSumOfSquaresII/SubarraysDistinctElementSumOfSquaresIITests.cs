using DSAExperimentation.LeetCode.SubarraysDistinctElementSumOfSquaresII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubarraysDistinctElementSumOfSquaresII;

// Harness only: the algorithms live in SubarraysDistinctElementSumOfSquaresIISolution.
// One test method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke.
public sealed partial class SubarraysDistinctElementSumOfSquaresIITests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [1, 2, 1], 15 },
            { [2, 2], 3 },
            { [1], 1 },
            { [1, 2, 3], 20 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumOfSquaresByBruteForce_LeetCodeExamples_ReturnsModuloSumOfSquares(int[] nums, long expected)
    {
        var actual = SubarraysDistinctElementSumOfSquaresIISolution.SumOfSquaresByBruteForce(nums);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumOfSquaresByRangeFenwickTree_LeetCodeExamples_ReturnsModuloSumOfSquares(int[] nums, long expected)
    {
        var actual = SubarraysDistinctElementSumOfSquaresIISolution.SumOfSquaresByRangeFenwickTree(nums);

        Assert.Equal(expected, actual);
    }
}
