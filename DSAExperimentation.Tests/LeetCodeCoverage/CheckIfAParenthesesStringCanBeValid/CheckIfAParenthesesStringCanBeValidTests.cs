using RepoIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfAParenthesesStringCanBeValid;

// LeetCode 2116. Check if a Parentheses String Can Be Valid: the same two-stack
// greedy ValidParenthesisStringTests (LC 678) already proved out for '*' wildcards
// applies unchanged here - locked[i] == '0' makes position i free to become either
// bracket, exactly the role '*' plays there. This repo's own Stack<int>
// (RepoIndexStack, ValidParenthesisStringTests precedent) tracks unmatched '('
// indices and unmatched free-position indices; each ')' first consumes an open
// paren, falling back to a free position, and any opens still unmatched afterward
// are paired against free positions after them, greedily from the innermost pair
// out.
public sealed partial class CheckIfAParenthesesStringCanBeValidTests
{
    [Theory]
    [InlineData("))()))", "010100", true)]
    [InlineData("()()", "0000", true)]
    [InlineData(")", "0", false)]
    public void CanBeValid_LeetCodeExamples_ReturnsExpectedValidity(string s, string locked, bool expected)
    {
        var actual = CanBeValid(s, locked);

        Assert.Equal(expected, actual);
    }

    private static bool CanBeValid(string s, string locked)
    {
        if (s.Length % 2 != 0)
        {
            return false;
        }

        if (!TryMatchClosingParens(s, locked, out var openIndices, out var freeIndices))
        {
            return false;
        }

        return ReconcileLeftoverOpens(openIndices, freeIndices);
    }

    private static bool TryMatchClosingParens(
        string s, string locked, out RepoIndexStack openIndices, out RepoIndexStack freeIndices)
    {
        openIndices = new RepoIndexStack();
        freeIndices = new RepoIndexStack();

        for (var i = 0; i < s.Length; i++)
        {
            if (locked[i] == '0')
            {
                freeIndices.Push(i);
            }
            else if (s[i] == '(')
            {
                openIndices.Push(i);
            }
            else if (!openIndices.TryPop(out _) && !freeIndices.TryPop(out _))
            {
                return false;
            }
        }

        return true;
    }

    private static bool ReconcileLeftoverOpens(RepoIndexStack openIndices, RepoIndexStack freeIndices)
    {
        while (openIndices.TryPop(out var openIndex))
        {
            if (!freeIndices.TryPop(out var freeIndex) || freeIndex < openIndex)
            {
                return false;
            }
        }

        return true;
    }
}
