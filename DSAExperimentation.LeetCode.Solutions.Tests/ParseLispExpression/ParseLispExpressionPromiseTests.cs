using DSAExperimentation.LeetCode.Solutions.ParseLispExpression;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ParseLispExpression;

public sealed partial class ParseLispExpressionPromiseTests
{
    public static TheoryData<string, bool> Inputs =>
        new()
        {
            { "(let x 2 (mult x (let x 3 y 4 (add x y))))", true },
            { "(let x 3 x 2 x)", true },
            { "(let x 1 y 2 x (add x y) (add x y))", true },
            { "-12", true },
            { "(let a1 5 (mult a1 -3))", true },
            { "(let x 1 (let y x (add x y)))", true },
            { "(let x 7 (add x 3) (mult x 2))", false },
            { "(let x 0 (let x 1 x) x)", false },
            { "(let x 1 (add x (let x 2 (add x 3)))", false },
            { "(add 1 2))", false },
            { "(add 1 2)(add 3 4)", false },
            { "(let x 1 (add x y))", false },
            { "(add (let y 2 y) y)", false },
            { "(let x (add x 1) x)", false },
            { "x", false },
            { "(add 1 2 3)", false },
            { "(let x 1)", false },
            { "(let add 1 add)", false },
            { "(sub 1 2)", false },
            { "(add  1 2)", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForALegalExpression(string expression, bool expected)
        => Assert.Equal(expected, ParseLispExpressionPromise.IsKeptBy(expression));
}
