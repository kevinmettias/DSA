using DSAExperimentation.LeetCode.Solutions.CheckIfAParenthesesStringCanBeValid;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CheckIfAParenthesesStringCanBeValid;

public sealed partial class CheckIfAParenthesesStringCanBeValidPromiseTests
{
    public static TheoryData<string, string, bool> Inputs =>
        new()
        {
            { "))()))", "010100", true },
            { "()()", "0000", true },
            { "(()", "11", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheStringAndItsMaskAreOneLength(string text, string digits, bool expected)
        => Assert.Equal(expected, CheckIfAParenthesesStringCanBeValidPromise.IsKeptBy(new(text), new(digits)));
}
