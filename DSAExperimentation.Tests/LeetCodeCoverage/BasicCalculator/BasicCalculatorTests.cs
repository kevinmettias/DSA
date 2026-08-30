using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<(int Result, int Sign)>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BasicCalculator;

// LeetCode 224. Basic Calculator: a single left-to-right pass over '+', '-', '(',
// ')', digits and spaces. Each '(' pushes the (result-so-far, sign) pair this
// repo's own Stack<(int,int)> is holding so far and resets both for the nested
// scope; each ')' pops that pair back and folds the nested result into it -
// no recursion, no hand-rolled parser stack.
public sealed partial class BasicCalculatorTests
{
    [Theory]
    [InlineData("1 + 1", 2)]
    [InlineData(" 2-1 + 2 ", 3)]
    [InlineData("(1+(4+5+2)-3)+(6+8)", 23)]
    [InlineData("2-(5-6)", 3)]
    public void Calculate_LeetCodeExamples_ReturnsEvaluatedResult(string expression, int expected)
        => Assert.Equal(expected, Calculate(expression));

    private static int Calculate(string expression)
    {
        var stack = new RepoStack();
        var result = 0;
        var sign = 1;
        var i = 0;

        while (i < expression.Length)
        {
            var c = expression[i];

            if (char.IsDigit(c))
            {
                var number = 0;

                while (i < expression.Length && char.IsDigit(expression[i]))
                {
                    number = number * 10 + (expression[i] - '0');
                    i++;
                }

                result += sign * number;
                continue;
            }

            switch (c)
            {
                case '+':
                    sign = 1;
                    break;
                case '-':
                    sign = -1;
                    break;
                case '(':
                    stack.Push((result, sign));
                    result = 0;
                    sign = 1;
                    break;
                case ')':
                    stack.TryPop(out var outer);
                    result = outer.Result + outer.Sign * result;
                    break;
            }

            i++;
        }

        return result;
    }
}
