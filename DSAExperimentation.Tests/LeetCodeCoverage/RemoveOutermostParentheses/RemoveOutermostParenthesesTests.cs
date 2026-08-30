using System.Text;
using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveOutermostParentheses;

// LeetCode 1021. Remove Outermost Parentheses: this repo's own Stack<char>
// (ValidParenthesesTests/MinimumAddToMakeParenthesesValidTests precedent) tracks
// how deeply nested the current primitive is - its own Count doubles as the
// depth counter, the same "stack size as depth" trick ScoreOfParenthesesTests
// uses. An opener is only emitted once something is already open (so the
// outermost opener of each primitive is skipped), and a closer pops first so
// its own post-pop Count is what excludes the outermost closer symmetrically.
public sealed partial class RemoveOutermostParenthesesTests
{
    [Fact]
    public void RemoveOuterParentheses_SingleNestedPrimitive_StripsOnlyOutermostPair()
        => Assert.Equal("()()", RemoveOuterParentheses("(()())"));

    [Fact]
    public void RemoveOuterParentheses_TwoSiblingPrimitives_StripsEachOutermostPair()
        => Assert.Equal("()()()", RemoveOuterParentheses("(()())(())"));

    [Fact]
    public void RemoveOuterParentheses_OnlyTrivialPrimitives_ReturnsEmptyString()
        => Assert.Equal(string.Empty, RemoveOuterParentheses("()()"));

    private static string RemoveOuterParentheses(string s)
    {
        var openers = new RepoCharStack();
        var result = new StringBuilder();

        foreach (var c in s)
        {
            if (c == '(')
            {
                if (openers.Count > 0)
                {
                    result.Append(c);
                }

                openers.Push(c);
            }
            else
            {
                openers.TryPop(out _);

                if (openers.Count > 0)
                {
                    result.Append(c);
                }
            }
        }

        return result.ToString();
    }
}
