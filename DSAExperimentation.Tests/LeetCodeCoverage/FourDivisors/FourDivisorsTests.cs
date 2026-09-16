using DSAExperimentation.LeetCode.FourDivisors;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FourDivisors;

// Harness only. Both strategies are FourDivisorsSolution's - the sqrt-anchored
// downward walk and the full-range trial division that used to live untested as the
// benchmark's baseline - pinned to LeetCode's published examples plus numbers with
// one, two, three and five divisors, which must all contribute nothing.
public sealed partial class FourDivisorsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [21, 4, 7], 32 },
            { [21, 21], 64 },
            { [1], 0 },
            { [8], 15 },
            { [6], 12 },
            { [2, 3, 5], 0 },
            { [16], 0 },
            { [9, 8], 15 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumFourDivisorsByBinarySearchAnchor_LeetCodeExamples_ReturnsSumOverQualifyingNumbers(
        int[] nums, int expected) =>
        Assert.Equal(expected, FourDivisorsSolution.SumFourDivisorsByBinarySearchAnchor(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumFourDivisorsByFullRangeScan_LeetCodeExamples_ReturnsSumOverQualifyingNumbers(
        int[] nums, int expected) =>
        Assert.Equal(expected, FourDivisorsSolution.SumFourDivisorsByFullRangeScan(nums));
}
