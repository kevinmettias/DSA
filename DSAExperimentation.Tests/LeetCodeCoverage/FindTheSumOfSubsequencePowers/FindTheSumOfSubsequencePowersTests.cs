using DSAExperimentation.LeetCode.FindTheSumOfSubsequencePowers;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheSumOfSubsequencePowers;

// Harness only. Both the exhaustive walk and the threshold-counting DP are
// FindTheSumOfSubsequencePowersSolution's - this file just pins them to LeetCode's
// published examples, including the zero-power duplicate-value case and a
// negative-value case.
public sealed class FindTheSumOfSubsequencePowersTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [1, 2, 3, 4], 3, 4 },
            { [2, 2], 2, 0 },
            { [4, 3, -1], 2, 10 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumOfPowersByBruteForce_LeetCodeExamples_ReturnsSumOfPowersModTenToTheNinePlusSeven(
        int[] nums, int k, int expected)
    {
        var actual = FindTheSumOfSubsequencePowersSolution.SumOfPowersByBruteForce(nums, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumOfPowersByThresholdCounting_LeetCodeExamples_ReturnsSumOfPowersModTenToTheNinePlusSeven(
        int[] nums, int k, int expected)
    {
        var actual = FindTheSumOfSubsequencePowersSolution.SumOfPowersByThresholdCounting(nums, k);

        Assert.Equal(expected, actual);
    }
}
