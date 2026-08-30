using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ScoreOfParentheses;

// LeetCode 856. Score of Parentheses: a single left-to-right pass over this
// repo's own Stack<int> of partial scores (DailyTemperatures/AsteroidCollision/
// BasicCalculatorTests precedent for this repo's own Stack instead of the CLR's
// own System.Collections.Generic.Stack), seeded with one sentinel 0 for the
// implicit outermost scope. Each '(' opens a fresh nested scope (push 0); each
// ')' closes the innermost open scope, folding its own score - 1 if it was
// empty, doubled otherwise - back into the score of the scope enclosing it.
// The sentinel is what makes that fold-back safe even when ')' closes the
// outermost pair, with no separate "am I at the top level" branch needed.
public sealed partial class ScoreOfParenthesesTests
{
    [Fact]
    public void Score_SinglePair_ReturnsOne()
        => Assert.Equal(1, Score("()"));

    [Fact]
    public void Score_NestedPair_ReturnsTwo()
        => Assert.Equal(2, Score("(())"));

    [Fact]
    public void Score_TwoSiblingPairs_ReturnsTwo()
        => Assert.Equal(2, Score("()()"));

    [Fact]
    public void Score_MixedNestingAndSiblings_ReturnsSix()
        => Assert.Equal(6, Score("(()(()))"));

    private static int Score(string s)
    {
        var scores = new RepoIntStack();
        scores.Push(0);

        foreach (var c in s)
        {
            if (c == '(')
            {
                scores.Push(0);
                continue;
            }

            scores.TryPop(out var inner);
            scores.TryPop(out var outer);
            scores.Push(outer + Math.Max(2 * inner, 1));
        }

        scores.TryPop(out var result);
        return result;
    }
}
