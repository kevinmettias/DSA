using System.Text;
using DSAExperimentation.LeetCode.Solutions.ParsingABooleanExpression;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ParsingABooleanExpression;

public sealed partial class ParsingABooleanExpressionPromiseTests
{
    // Each '!(' and its ')' cost three characters, so a leaf under this many is 19,999 characters
    // long: the deepest nesting the length bound admits. One level more makes 20,002.
    private const int DeepestNesting = 6_666;

    public static TheoryData<string, bool> Inputs =>
        new()
        {
            { "&(|(f))", true },
            { "|(f,f,f,t)", true },
            { "!(&(f,t))", true },
            { "t", true },
            { Negated(DeepestNesting), true },
            { Negated(DeepestNesting + 1), false },
            { "!(&(f,t),|(t,t))", false },
            { "|(|(t),f", false },
            { "|(t,f),!(t)", false },
            { "t)", false },
            { "&()", false },
            { "&(t,)", false },
            { "&t", false },
            { "(t)", false },
            { "&(t, f)", false },
            { "", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForAnExpressionTheGrammarDerivesWithinTheBound(string expression, bool expected)
        => Assert.Equal(expected, ParsingABooleanExpressionPromise.IsKeptBy(expression));

    private static string Negated(int depth)
        => new StringBuilder().Insert(0, "!(", depth).Append('t').Append(')', depth).ToString();
}
