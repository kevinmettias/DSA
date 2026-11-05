using DSAExperimentation.LeetCode.Solutions.BasicCalculatorIV;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BasicCalculatorIV;

public sealed partial class BasicCalculatorIVPromiseTests
{
    public static TheoryData<string, string[], int[], bool> Inputs =>
        new()
        {
            { "e + 8 - a + 5", ["e"], [1], true },
            { "(e + 8) * (e - 8)", [], [], true },
            { "a + b * c - d / e", ["a"], [1], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheExpressionUsesOnlyItsStatedCharacters(string expression, string[] evalvars, int[] evalints, bool expected)
        => Assert.Equal(expected, BasicCalculatorIVPromise.IsKeptBy(expression, evalvars, evalints));
}
