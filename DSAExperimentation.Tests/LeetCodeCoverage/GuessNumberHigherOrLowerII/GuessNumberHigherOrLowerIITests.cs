using DSAExperimentation.LeetCode.GuessNumberHigherOrLowerII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GuessNumberHigherOrLowerII;

// Harness only: both strategies live in GuessNumberHigherOrLowerIISolution. One test
// method per strategy over one shared set of LeetCode's own examples, so a failure
// names the strategy that broke.
public sealed class GuessNumberHigherOrLowerIITests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 1, 0 },
            { 2, 1 },
            { 10, 16 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetMoneyAmountByUnmemoizedRecursion_LeetCodeExamples_ReturnsMinimumGuaranteedMoney(
        int n, int expected) =>
        Assert.Equal(expected, GuessNumberHigherOrLowerIISolution.GetMoneyAmountByUnmemoizedRecursion(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetMoneyAmountByMemoizedRecursion_LeetCodeExamples_ReturnsMinimumGuaranteedMoney(
        int n, int expected) =>
        Assert.Equal(expected, GuessNumberHigherOrLowerIISolution.GetMoneyAmountByMemoizedRecursion(n));
}
