using DSAExperimentation.LeetCode.NumberOfBeautifulIntegersInTheRange;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfBeautifulIntegersInTheRange;

// Harness only: the algorithms live in NumberOfBeautifulIntegersInTheRangeSolution.
// One test method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke.
public sealed class NumberOfBeautifulIntegersInTheRangeTests
{
    public static TheoryData<int, int, int, long> Examples =>
        new()
        {
            { 10, 20, 3, 2 }, // 12, 18: one even + one odd digit each, both divisible by 3
            { 1, 10, 1, 1 }, // only 10 (digits 1,0 - one odd, one even)
            { 5, 5, 2, 0 }, // 5 is a single odd digit (never balanced) and isn't even divisible by 2
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBruteForce_LeetCodeExamples_ReturnsBeautifulCount(
        int low, int high, int divisor, long expected)
    {
        var actual = NumberOfBeautifulIntegersInTheRangeSolution.CountByBruteForce(low, high, divisor);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByDigitDpMemo_LeetCodeExamples_ReturnsBeautifulCount(
        int low, int high, int divisor, long expected)
    {
        var actual = NumberOfBeautifulIntegersInTheRangeSolution.CountByDigitDpMemo(low, high, divisor);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1, 1000, 1)]
    [InlineData(123, 4567, 7)]
    [InlineData(1, 999_999, 13)]
    public void BothStrategies_RandomizedRanges_Agree(int low, int high, int divisor)
    {
        var bruteForce = NumberOfBeautifulIntegersInTheRangeSolution.CountByBruteForce(low, high, divisor);
        var digitDp = NumberOfBeautifulIntegersInTheRangeSolution.CountByDigitDpMemo(low, high, divisor);

        Assert.Equal(bruteForce, digitDp);
    }
}
