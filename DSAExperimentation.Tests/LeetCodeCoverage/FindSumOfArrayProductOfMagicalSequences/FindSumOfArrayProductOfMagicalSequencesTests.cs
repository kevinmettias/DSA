using DSAExperimentation.LeetCode.FindSumOfArrayProductOfMagicalSequences;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindSumOfArrayProductOfMagicalSequences;

// Harness only. Both the direct enumeration and the carry-digit DP are
// FindSumOfArrayProductOfMagicalSequencesSolution's - this file just pins them to
// LeetCode's published examples.
public sealed class FindSumOfArrayProductOfMagicalSequencesTests
{
    public static TheoryData<int, int, int[], int> Examples =>
        new()
        {
            { 5, 5, [1, 10, 100, 10_000, 1_000_000], 991_600_007 },
            { 2, 2, [5, 4, 3, 2, 1], 170 },
            { 1, 1, [28], 28 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumOfProductsByBacktrackEnumeration_LeetCodeExamples_ReturnsSumOfProductsModulo1e9Plus7(
        int sequenceLength, int requiredSetBits, int[] nums, int expected)
    {
        var actual = FindSumOfArrayProductOfMagicalSequencesSolution.SumOfProductsByBacktrackEnumeration(
            sequenceLength, requiredSetBits, nums);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumOfProductsByCarryDigitDp_LeetCodeExamples_ReturnsSumOfProductsModulo1e9Plus7(
        int sequenceLength, int requiredSetBits, int[] nums, int expected)
    {
        var actual = FindSumOfArrayProductOfMagicalSequencesSolution.SumOfProductsByCarryDigitDp(
            sequenceLength, requiredSetBits, nums);

        Assert.Equal(expected, actual);
    }
}
