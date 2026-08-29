using IndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestValidParentheses;

public sealed partial class LongestValidParenthesesTests
{
    [Theory]
    [InlineData("(()", 2)]
    [InlineData(")()())", 4)]
    [InlineData("", 0)]
    public void LongestValidParentheses_LeetCodeExamples_ReturnsLongestValidSpan(string value, int expected)
        => Assert.Equal(expected, LongestValidParentheses(value));

    private static int LongestValidParentheses(string value)
    {
        var stack = new IndexStack();
        stack.Push(-1);
        var best = 0;
        for (var i = 0; i < value.Length; i++)
        {
            if (value[i] == '(')
            {
                stack.Push(i);
            }
            else
            {
                stack.TryPop(out _);
                if (stack.TryPeek(out var start)) best = Math.Max(best, i - start);
                else stack.Push(i);
            }
        }
        return best;
    }
}
