using DSAExperimentation.LeetCode.ProductOfArrayExceptSelf;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ProductOfArrayExceptSelf;

// Harness only. The prefix/suffix product pass is
// ProductOfArrayExceptSelfSolution's - this file just pins it to LeetCode's
// published examples, including the zero-operand case a division-based
// approach could not answer.
public sealed class ProductOfArrayExceptSelfTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4], [24, 12, 8, 6] },
            { [-1, 1, 0, -3, 3], [0, 0, 9, 0, 0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ProductExceptSelfByPrefixSuffixPass_LeetCodeExamples_ReturnsProducts(
        int[] nums, int[] expected) =>
        Assert.Equal(expected, ProductOfArrayExceptSelfSolution.ProductExceptSelfByPrefixSuffixPass(nums));
}
