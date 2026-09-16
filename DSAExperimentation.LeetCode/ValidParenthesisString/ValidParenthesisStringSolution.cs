using RepoIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.ValidParenthesisString;

// LeetCode 678. Valid Parenthesis String: a string of '(', ')' and '*' (a
// wildcard standing for '(', ')' or "") is valid if some substitution of every
// '*' makes the parentheses balanced.
internal static class ValidParenthesisStringSolution
{
    // The textbook answer: track every open-paren count reachable at each
    // position in a plain BCL HashSet<int> - the "one interpretation per '*'"
    // state explosion this problem is famous for, O(n^2) worst case since the
    // reachable set can grow by one value per character. Deliberately written
    // without this repo's primitives - it is the arm the composed solution below
    // has to justify itself against.
    public static bool IsValidStringByReachableOpenCountDp(string text)
    {
        var reachable = new HashSet<int> { 0 };

        foreach (var symbol in text)
        {
            reachable = ComputeNextReachable(reachable, symbol);

            if (reachable.Count == 0)
            {
                return false;
            }
        }

        return reachable.Contains(0);
    }

    private static HashSet<int> ComputeNextReachable(HashSet<int> reachable, char symbol)
    {
        var next = new HashSet<int>();

        foreach (var openCount in reachable)
        {
            switch (symbol)
            {
                case '(':
                    AddOpenParenTransition(next, openCount);
                    break;
                case ')':
                    AddCloseParenTransition(next, openCount);
                    break;
                default:
                    AddWildcardTransition(next, openCount);
                    break;
            }
        }

        return next;
    }

    private static void AddOpenParenTransition(HashSet<int> next, int openCount) => next.Add(openCount + 1);

    private static void AddCloseParenTransition(HashSet<int> next, int openCount)
    {
        if (openCount > 0)
        {
            next.Add(openCount - 1);
        }
    }

    private static void AddWildcardTransition(HashSet<int> next, int openCount)
    {
        next.Add(openCount + 1);
        next.Add(openCount);

        if (openCount > 0)
        {
            next.Add(openCount - 1);
        }
    }

    // This repo's own two Stack<int> (ValidParenthesesTests/NextGreaterElementITests
    // precedent for Stack<char>/Stack<int>) track the indices of unmatched '(' and
    // unmatched '*' in one O(n) left-to-right pass. Each ')' first consumes an open
    // paren, falling back to a wildcard; any opens still unmatched afterward are
    // then paired against wildcards positioned after them, greedily from the
    // innermost pair out.
    public static bool IsValidStringByTwoIndexStackSweep(string text)
    {
        var openIndices = new RepoIndexStack();
        var starIndices = new RepoIndexStack();

        return TryMatchClosingParens(text, openIndices, starIndices) &&
               IsEveryOpenMatched(openIndices, starIndices);
    }

    private static bool TryMatchClosingParens(string text, RepoIndexStack openIndices, RepoIndexStack starIndices)
    {
        for (var i = 0; i < text.Length; i++)
        {
            switch (text[i])
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

        return true;
    }

    private static bool IsEveryOpenMatched(RepoIndexStack openIndices, RepoIndexStack starIndices)
    {
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
