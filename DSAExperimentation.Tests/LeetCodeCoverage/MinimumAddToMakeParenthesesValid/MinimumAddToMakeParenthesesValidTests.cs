using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumAddToMakeParenthesesValid;

// LeetCode 921. Minimum Add to Make Parentheses Valid: this repo's Stack<char>
// tracks unmatched openers exactly the way ValidParenthesesTests does; every
// closer with nothing to pop against is itself an unmatched closer needing an
// inserted opener, and whatever openers remain on the stack at the end each need
// an inserted closer.
public sealed partial class MinimumAddToMakeParenthesesValidTests
{
    [Fact]
    public void MinAddToMakeValid_UnmatchedClosers_ReturnsInsertionsNeeded() => Assert.Equal(1, MinAddToMakeValid("())"));

    [Fact]
    public void MinAddToMakeValid_UnmatchedOpeners_ReturnsInsertionsNeeded() => Assert.Equal(3, MinAddToMakeValid("((("));

    [Fact]
    public void MinAddToMakeValid_AlreadyValid_ReturnsZero() => Assert.Equal(0, MinAddToMakeValid("()"));

    [Fact]
    public void MinAddToMakeValid_UnmatchedOnBothSides_ReturnsCombinedInsertions() => Assert.Equal(4, MinAddToMakeValid("()))(("));

    private static int MinAddToMakeValid(string s)
    {
        var openers = new RepoCharStack();
        var unmatchedClosers = 0;

        foreach (var ch in s)
        {
            if (ch == '(')
            {
                openers.Push(ch);
            }
            else if (!openers.TryPop(out _))
            {
                unmatchedClosers++;
            }
        }

        return unmatchedClosers + openers.Count;
    }
}
