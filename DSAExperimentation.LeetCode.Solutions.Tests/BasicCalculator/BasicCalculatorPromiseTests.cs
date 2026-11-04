using DSAExperimentation.LeetCode.Solutions.BasicCalculator;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BasicCalculator;

public sealed partial class BasicCalculatorPromiseTests
{
    public static TheoryData<string, bool> Inputs =>
        new()
        {
            { "1 + 1", true },
            { "(1+(4+5+2)-3)+(6+8)", true },
            { "100 * 2 + 12", false },
            { "100 + ((-200 + 300)", false },
            { "1 + 2)", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForABalancedSumOfDifferences(string expression, bool expected)
        => Assert.Equal(expected, BasicCalculatorPromise.IsKeptBy(expression));
}
