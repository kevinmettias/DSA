using RepoCharListStack = DSAExperimentation.DataStructures.Stack.Stack<System.Collections.Generic.List<char>>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReverseSubstringsBetweenEachPairOfParentheses;

// LeetCode 1190. Reverse Substrings Between Each Pair of Parentheses: this repo's own
// Stack<List<char>> holds one "in progress" character buffer per currently-open
// paren; '(' pushes a fresh buffer, ')' reverses the current buffer in place and
// folds it into the buffer beneath it on the stack, so every nesting level is
// reversed exactly once, from the inside out.
public sealed partial class ReverseSubstringsBetweenEachPairOfParenthesesTests
{
    [Fact]
    public void ReverseParentheses_NestedParentheses_ReversesInnermostFirst()
        => Assert.Equal("leetcode", ReverseParentheses("(ed(et(oc))el)"));

    [Fact]
    public void ReverseParentheses_SingleGroup_ReversesGroup()
        => Assert.Equal("iloveu", ReverseParentheses("(u(love)i)"));

    [Fact]
    public void ReverseParentheses_NoParentheses_ReturnsInputUnchanged()
        => Assert.Equal("abc", ReverseParentheses("abc"));

    private static string ReverseParentheses(string s)
    {
        var groups = new RepoCharListStack();
        var current = new List<char>();

        foreach (var ch in s)
        {
            switch (ch)
            {
                case '(':
                    groups.Push(current);
                    current = [];
                    break;
                case ')':
                    current.Reverse();
                    if (groups.TryPop(out var enclosing))
                    {
                        enclosing.AddRange(current);
                        current = enclosing;
                    }

                    break;
                default:
                    current.Add(ch);
                    break;
            }
        }

        return new string(current.ToArray());
    }
}
