using DSAExperimentation.LeetCode.SumOfBeautifulSubsequences;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfBeautifulSubsequences;

// Harness only. Both strategies are SumOfBeautifulSubsequencesSolution's - this
// file just pins them to LeetCode's published examples.
public sealed partial class SumOfBeautifulSubsequencesTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 3], 10 },
            { [4, 6], 12 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumBeautyByBruteForce_LeetCodeExamples_ReturnsSumOfBeautyValues(int[] nums, int expected) =>
        Assert.Equal(expected, SumOfBeautifulSubsequencesSolution.SumBeautyByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumBeautyByDivisorSieve_LeetCodeExamples_ReturnsSumOfBeautyValues(int[] nums, int expected) =>
        Assert.Equal(expected, SumOfBeautifulSubsequencesSolution.SumBeautyByDivisorSieve(nums));
}
