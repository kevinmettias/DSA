using OpenerStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.MinimumAddToMakeParenthesesValid;

// LeetCode 921. Minimum Add to Make Parentheses Valid: the fewest parentheses that
// have to be inserted anywhere in the string to make it balanced.
//
// One walk answers it either way: a closer with nothing open in front of it is
// itself unmatched and needs an inserted opener, and whatever openers are still
// unclosed at the end each need an inserted closer. The two strategies differ only
// in what they keep about those pending openers - a running count, or the openers
// themselves on this repo's own Stack<char>.
internal static class MinimumAddToMakeParenthesesValidSolution
{
    private const char Opener = '(';

    // The textbook answer: a pair of running counters, nothing stored.
    // Deliberately written without this repo's primitives - it is the arm the
    // stack strategy below has to justify itself against.
    public static int MinAddToMakeValidByRunningCounter(string text)
    {
        var openBalance = 0;
        var insertions = 0;

        foreach (var ch in text)
        {
            if (ch == Opener)
            {
                openBalance++;
            }
            else if (openBalance > 0)
            {
                openBalance--;
            }
            else
            {
                insertions++;
            }
        }

        return insertions + openBalance;
    }

    // This repo's own LIFO primitive holding each unmatched opener explicitly, the
    // same composition ValidParenthesesSolution uses: TryPop failing is exactly
    // "this closer has nothing to match", and Count at the end is exactly how many
    // openers are still waiting for one.
    public static int MinAddToMakeValidByOpenerStack(string text)
    {
        var openers = new OpenerStack();
        var unmatchedClosers = 0;

        foreach (var ch in text)
        {
            if (ch == Opener)
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
