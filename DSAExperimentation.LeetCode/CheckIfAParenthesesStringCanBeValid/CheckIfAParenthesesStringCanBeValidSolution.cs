using RepoIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.CheckIfAParenthesesStringCanBeValid;

// LeetCode 2116. Check if a Parentheses String Can Be Valid: every position whose
// locked digit is '0' may be rewritten to either bracket, every position whose digit
// is '1' is fixed. Report whether some rewriting makes the string balanced.
//
// A free position plays the role '*' plays in LC 678 Valid Parenthesis String, with
// one difference the DP arm below has to respect: a free position must become a
// bracket, it can never vanish. An odd-length string is therefore never valid.
internal static class CheckIfAParenthesesStringCanBeValidSolution
{
    private const char OpenParenthesis = '(';
    private const char FreePosition = '0'; // locked[i] == '0' -> position i may become either bracket

    // The textbook answer: carry the whole set of open-paren counts still reachable
    // after each position and check whether zero survives to the end. Deliberately
    // BCL - a HashSet<int> rebuilt per position - so it is the arm the single-pass
    // sweep below has to beat. Its reachable set can grow with the string, making it
    // O(n^2) where the sweep is O(n). No parity guard is needed: after an odd number
    // of positions every reachable count is odd, so zero cannot survive.
    public static bool CanBeValidByReachableOpenCountDp(ParenthesisString s, LockMask locked)
    {
        var reachable = new HashSet<int> { 0 };

        for (var i = 0; i < s.Text.Length; i++)
        {
            reachable = ComputeNextReachable(reachable, s.Text[i], locked.Digits[i]);

            if (reachable.Count == 0)
            {
                return false;
            }
        }

        return reachable.Contains(0);
    }

    private static HashSet<int> ComputeNextReachable(HashSet<int> reachable, char c, char lockedChar)
    {
        var next = new HashSet<int>();

        foreach (var openCount in reachable)
        {
            if (lockedChar == FreePosition)
            {
                AddFreeTransition(next, openCount);
            }
            else if (c == OpenParenthesis)
            {
                next.Add(openCount + 1);
            }
            else if (openCount > 0)
            {
                next.Add(openCount - 1);
            }
        }

        return next;
    }

    private static void AddFreeTransition(HashSet<int> next, int openCount)
    {
        next.Add(openCount + 1);

        if (openCount > 0)
        {
            next.Add(openCount - 1);
        }
    }

    // One left-to-right pass over two of this repo's own Stack<int>: unmatched
    // locked '(' indices in one, unmatched free-position indices in the other. Each
    // locked ')' consumes an open paren first, falling back to a free position; any
    // opens still unmatched afterwards are paired against free positions that come
    // after them, greedily from the innermost pair out. The length parity check is
    // load-bearing here - without it a lone free position reports valid, because
    // nothing is left on either stack to contradict it.
    public static bool CanBeValidByIndexStackSweep(ParenthesisString s, LockMask locked)
    {
        if (s.Text.Length % 2 != 0)
        {
            return false;
        }

        var openIndices = new RepoIndexStack();
        var freeIndices = new RepoIndexStack();

        if (!TryMatchClosingParens(s, locked, openIndices, freeIndices))
        {
            return false;
        }

        return AllOpensMatched(openIndices, freeIndices);
    }

    private static bool TryMatchClosingParens(
        ParenthesisString s, LockMask locked, RepoIndexStack openIndices, RepoIndexStack freeIndices)
    {
        for (var i = 0; i < s.Text.Length; i++)
        {
            if (locked.Digits[i] == FreePosition)
            {
                freeIndices.Push(i);
            }
            else if (s.Text[i] == OpenParenthesis)
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

    private static bool AllOpensMatched(RepoIndexStack openIndices, RepoIndexStack freeIndices)
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

    // LC 2116's two operands, named for the roles they play here rather than left as two
    // adjacent `string` positions a caller could hand over the wrong way round with the
    // compiler none the wiser. `s` is the bracket string itself; `locked` is the
    // per-position lock digit string that decides which characters may still move. The
    // two hold different alphabets, so a swap is a silently wrong answer rather than a
    // different question.
    internal readonly record struct ParenthesisString(string Text);

    internal readonly record struct LockMask(string Digits);
}
