using DSAExperimentation.LeetCode.Solutions.EvaluateReversePolishNotation;

namespace DSAExperimentation.LeetCode.Solutions.Tests.EvaluateReversePolishNotation;

public sealed partial class EvaluateReversePolishNotationPromiseTests
{
    public static TheoryData<string[], bool> Inputs =>
        new()
        {
            { ["2", "1", "+", "3", "*"], true },
            { ["4", "-13", "5", "/", "+"], true },
            { ["+"], false },
            { ["1", "2", "3", "+"], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForOneValidExpression(string[] tokens, bool expected)
        => Assert.Equal(expected, EvaluateReversePolishNotationPromise.IsKeptBy(tokens));
}
