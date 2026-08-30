using RepoIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidParenthesisString;

// LeetCode 678. Valid Parenthesis String: two of this repo's own Stack<int>
// (ValidParenthesesTests/NextGreaterElementITests precedent for Stack<char>/Stack<int>)
// track the indices of unmatched '(' and unmatched '*'. Each ')' first consumes an
// open paren, falling back to a wildcard; any opens still unmatched afterward are then
// paired against wildcards positioned after them, greedily from the innermost pair out.
public sealed partial class ValidParenthesisStringTests
{
    [Theory]
    [InlineData("()", true)]
    [InlineData("(*)", true)]
    [InlineData("(*))", true)]
    [InlineData("(((", false)]
    public void CheckValidString_Examples_MatchesExpectedValidity(string s, bool expected) =>
        Assert.Equal(expected, CheckValidString(s));

    private static bool CheckValidString(string s)
    {
        var openIndices = new RepoIndexStack();
        var starIndices = new RepoIndexStack();

        for (var i = 0; i < s.Length; i++)
        {
            switch (s[i])
            {
                case '(':
                    openIndices.Push(i);
                    break;
                case '*':
                    starIndices.Push(i);
                    break;
                default:
                    if (!openIndices.TryPop(out _) && !starIndices.TryPop(out _))
                    {
                        return false;
                    }
                    break;
            }
        }

        while (openIndices.TryPop(out var openIndex))
        {
            if (!starIndices.TryPop(out var starIndex) || starIndex < openIndex)
            {
                return false;
            }
        }

        return true;
    }
}
