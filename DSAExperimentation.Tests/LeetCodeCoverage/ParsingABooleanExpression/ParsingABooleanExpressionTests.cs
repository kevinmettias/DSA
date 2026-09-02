using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ParsingABooleanExpression;

// LeetCode 1106. Parsing A Boolean Expression: this repo's Stack<char> as an explicit
// parser stack, the same "repo Stack instead of recursion" move DecodeStringTests and
// BasicCalculatorTests already make. 't'/'f'/operator characters are pushed as
// encountered; on every ')' the operands since the matching '(' are popped and
// tallied, the operator below that '(' is popped and applied, and the single 't'/'f'
// result is pushed back - so by the time the closing ')' of the outermost group is
// processed, exactly one value remains on the stack.
public sealed partial class ParsingABooleanExpressionTests
{
    [Theory]
    [InlineData("t", true)]
    [InlineData("f", false)]
    [InlineData("!(f)", true)]
    [InlineData("!(t)", false)]
    [InlineData("&(t,f)", false)]
    [InlineData("&(t,t,t)", true)]
    [InlineData("|(t,f)", true)]
    [InlineData("|(f,f,f)", false)]
    [InlineData("|(&(t,f,t),!(t))", false)]
    public void Parse_LeetCodeExamples_EvaluatesToExpectedBoolean(string expression, bool expected)
        => Assert.Equal(expected, Parse(expression));

    private static bool Parse(string expression)
    {
        var stack = new RepoCharStack();

        foreach (var c in expression)
        {
            if (c == ',')
            {
                continue;
            }

            if (c != ')')
            {
                stack.Push(c);
                continue;
            }

            stack.Push(EvaluateGroup(stack));
        }

        stack.TryPop(out var result);
        return result == 't';
    }

    // Pops operands back to (and including) the matching '(' plus the operator below
    // it, tallies how many were 't' vs. 'f', and returns the group's collapsed value.
    private static char EvaluateGroup(RepoCharStack stack)
    {
        var (trueCount, falseCount) = TallyOperands(stack);

        stack.TryPop(out _); // the matching '('
        stack.TryPop(out var op);

        var value = Evaluate(op, trueCount, falseCount);

        return value ? 't' : 'f';
    }

    private static (int TrueCount, int FalseCount) TallyOperands(RepoCharStack stack)
    {
        var trueCount = 0;
        var falseCount = 0;

        while (stack.TryPeek(out var top) && top != '(')
        {
            stack.TryPop(out var operand);
            if (operand == 't')
            {
                trueCount++;
            }
            else
            {
                falseCount++;
            }
        }

        return (trueCount, falseCount);
    }

    private static bool Evaluate(char op, int trueCount, int falseCount)
        => op switch
        {
            '!' => trueCount == 0,
            '&' => falseCount == 0,
            _ => trueCount > 0, // '|'
        };
}
