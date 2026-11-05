using DSAExperimentation.LeetCode.Solutions.MaximumNestingDepthOfTwoValidParenthesesStrings;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumNestingDepthOfTwoValidParenthesesStrings;

public sealed partial class MaximumNestingDepthOfTwoValidParenthesesStringsPromiseTests
{
    public static TheoryData<string, bool> Inputs =>
        new()
        {
            { "(()())", true },
            { "()", true },
            { "(()", false },
            { ")(", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForAValidParenthesesString(string seq, bool expected)
        => Assert.Equal(expected, MaximumNestingDepthOfTwoValidParenthesesStringsPromise.IsKeptBy(seq));
}
