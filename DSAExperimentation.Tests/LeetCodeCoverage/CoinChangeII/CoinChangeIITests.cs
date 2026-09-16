using DSAExperimentation.LeetCode.CoinChangeII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CoinChangeII;

// Harness only. Both strategies are CoinChangeIISolution's - this file just pins
// them to LeetCode's published examples, including the tabulation baseline, which
// was never asserted before this migration.
public sealed partial class CoinChangeIITests
{
    public static TheoryData<int, int[], int> Examples =>
        new()
        {
            { 5, [1, 2, 5], 4 },
            { 3, [2], 0 },
            { 10, [10], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountCombinationsByTabulation_LeetCodeExamples_ReturnsCombinationCount(
        int amount, int[] coins, int expected)
    {
        var actual = CoinChangeIISolution.CountCombinationsByTabulation(amount, coins);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountCombinationsByMemoizedTopDown_LeetCodeExamples_ReturnsCombinationCount(
        int amount, int[] coins, int expected)
    {
        var actual = CoinChangeIISolution.CountCombinationsByMemoizedTopDown(amount, coins);

        Assert.Equal(expected, actual);
    }
}
