using DSAExperimentation.LeetCode.Solutions.FourDivisors;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FourDivisors;

// Harness only. Both strategies are FourDivisorsSolution's - the sqrt-anchored
// downward walk and the full-range trial division that used to live untested as the
// benchmark's baseline - pinned to numbers with
// one, two, three and five divisors, which must all contribute nothing.
public sealed partial class FourDivisorsSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1], 0 },
            { [8], 15 },
            { [6], 12 },
            { [2, 3, 5], 0 },
            { [16], 0 },
            { [9, 8], 15 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SumFourDivisorsByBinarySearchAnchor_Cases_ReturnsSumOverQualifyingNumbers(
        int[] nums, int expected) =>
        Assert.Equal(expected, FourDivisorsSolution.SumFourDivisorsByBinarySearchAnchor(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void SumFourDivisorsByFullRangeScan_Cases_ReturnsSumOverQualifyingNumbers(
        int[] nums, int expected) =>
        Assert.Equal(expected, FourDivisorsSolution.SumFourDivisorsByFullRangeScan(nums));
}
