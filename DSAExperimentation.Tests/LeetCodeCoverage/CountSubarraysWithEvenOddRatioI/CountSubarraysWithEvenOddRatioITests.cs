using DSAExperimentation.LeetCode.CountSubarraysWithEvenOddRatioI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountSubarraysWithEvenOddRatioI;

// Harness only. Both strategies are CountSubarraysWithEvenOddRatioISolution's
// - this file just pins them to LeetCode's published examples.
public sealed partial class CountSubarraysWithEvenOddRatioITests
{
    public static TheoryData<int[], int, int, int> Examples =>
        new()
        {
            { [1, 2, 1, 2], 3, 2, 7 },
            { [2, 2, 1], 2, 1, 3 },
            { [2, 2, 2], 1, 1, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBruteForce_LeetCodeExamples_ReturnsValidSubarrayCount(
        int[] nums, int ratioNumerator, int ratioDenominator, int expected)
    {
        var actual = CountSubarraysWithEvenOddRatioISolution.CountByBruteForce(
            nums, ratioNumerator, ratioDenominator);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByFenwickPrefixSweep_LeetCodeExamples_ReturnsValidSubarrayCount(
        int[] nums, int ratioNumerator, int ratioDenominator, int expected)
    {
        var actual = CountSubarraysWithEvenOddRatioISolution.CountByFenwickPrefixSweep(
            nums, ratioNumerator, ratioDenominator);
        Assert.Equal(expected, actual);
    }
}
