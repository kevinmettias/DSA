using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNestingDepthOfTheParentheses;

// LeetCode 1614. Maximum Nesting Depth of the Parentheses: this repo's own
// Stack<char> (ValidParenthesesTests/RemoveOutermostParenthesesTests precedent)
// holds every unmatched '(' - its own Count doubles as the running depth, the same
// "stack size as depth" trick RemoveOutermostParenthesesTests uses, just reporting
// the running maximum instead of stripping the outermost pair. Digits and the four
// arithmetic operators pass through untouched.
public sealed partial class MaximumNestingDepthOfTheParenthesesTests
{
    [Fact]
    public void MaxDepth_MixedArithmeticAndNesting_ReturnsDeepestLevel()
        => Assert.Equal(3, MaxDepth("(1+(2*3)+((8)/4))+1"));

    [Fact]
    public void MaxDepth_ThreeSiblingGroupsSameDepth_ReturnsThatDepth()
        => Assert.Equal(3, MaxDepth("(1)+((2))+(((3)))"));

    [Fact]
    public void MaxDepth_NoParenthesesAtAll_ReturnsZero()
        => Assert.Equal(0, MaxDepth("1"));

    private static int MaxDepth(string s)
    {
        var openers = new RepoCharStack();
        var maxDepth = 0;

        foreach (var c in s)
        {
            if (c == '(')
            {
                openers.Push(c);
                maxDepth = Math.Max(maxDepth, openers.Count);
            }
            else if (c == ')')
            {
                openers.TryPop(out _);
            }
        }

        return maxDepth;
    }
}
