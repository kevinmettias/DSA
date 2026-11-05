using DSAExperimentation.LeetCode.Solutions.NumberOfBeautifulIntegersInTheRange;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfBeautifulIntegersInTheRange;

// Harness only: the algorithms live in NumberOfBeautifulIntegersInTheRangeSolution.
// One test method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke.
public sealed partial class NumberOfBeautifulIntegersInTheRangeSolutionTests
{
    public static TheoryData<int, int, int> RandomizedRanges =>
        new() { { 1, 1000, 1 }, { 123, 4567, 7 }, { 1, 999_999, 13 } };

    [Theory]
    [MemberData(nameof(RandomizedRanges))]
    public void BothStrategies_RandomizedRanges_Agree(int low, int high, int divisor)
    {
        var bruteForce = NumberOfBeautifulIntegersInTheRangeSolution.CountByBruteForce(low, high, divisor);
        var digitDp = NumberOfBeautifulIntegersInTheRangeSolution.CountByDigitDpMemo(low, high, divisor);

        Assert.Equal(bruteForce, digitDp);
    }
}
