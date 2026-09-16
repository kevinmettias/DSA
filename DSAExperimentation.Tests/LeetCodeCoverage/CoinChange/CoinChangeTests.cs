using DSAExperimentation.LeetCode.CoinChange;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CoinChange;

// Harness only: both strategies live in CoinChangeSolution and are asserted
// against the same examples.
public sealed class CoinChangeTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [1, 2, 5], 11, 3 },
            { [2], 3, -1 },
            { [1], 0, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FewestCoinsByTabulation_LeetCodeExamples_ReturnsMinimumCoinCount(
        int[] coins, int amount, int expected)
    {
        var actual = CoinChangeSolution.FewestCoinsByTabulation(coins, amount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FewestCoinsByMemoization_LeetCodeExamples_ReturnsMinimumCoinCount(
        int[] coins, int amount, int expected)
    {
        var actual = CoinChangeSolution.FewestCoinsByMemoization(coins, amount);

        Assert.Equal(expected, actual);
    }
}
