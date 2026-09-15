using IndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.LongestValidParentheses;

// LeetCode 32. Longest Valid Parentheses: length of the longest well-formed
// parenthesis substring.
//
// Both strategies scan left to right and differ only in how they track where
// the current valid run began - a classic O(n) DP array of "how far back does
// a valid run reach", or a stack of unmatched indices whose top always marks
// the boundary just before the current run.
internal static class LongestValidParenthesesSolution
{
    private const int MatchedPairLength = 2;

    // The textbook DP: dp[i] is the length of the valid substring ending at i.
    // Written without this repo's own Stack<T> - the arm StackScan has to beat.
    public static int LengthByDynamicProgrammingArray(string value)
    {
        var dp = new int[value.Length];
        var best = 0;

        for (var i = 1; i < value.Length; i++)
        {
            if (value[i] != ')')
            {
                continue;
            }

            var open = i - dp[i - 1] - 1;

            if (open >= 0 && value[open] == '(')
            {
                dp[i] = dp[i - 1] + MatchedPairLength + (open > 0 ? RunLengthBefore(dp, open) : 0);
                best = Math.Max(best, dp[i]);
            }
        }

        return best;
    }

    private static int RunLengthBefore(int[] dp, int open) => dp[open - 1];

    // A stack of unmatched indices, seeded with -1 so the first valid run's
    // length is measured from a real boundary. Popping on ')' either finds the
    // matching '(' (the new top marks the run's start) or, if the stack empties,
    // pushes the ')' itself as the new boundary.
    public static int LengthByStackScan(string value)
    {
        var stack = new IndexStack();
        stack.Push(-1);
        var best = 0;

        for (var i = 0; i < value.Length; i++)
        {
            if (value[i] == '(')
            {
                stack.Push(i);
                continue;
            }

            best = BestAfterClose(stack, i, best);
        }

        return best;
    }

    // One ')' of the scan: its matching '(' comes off the stack, so the new top
    // marks where the run it just closed began - or, if nothing is left to mark
    // one, this index becomes the boundary the next run is measured from.
    private static int BestAfterClose(IndexStack stack, int index, int best)
    {
        stack.TryPop(out _);

        if (stack.TryPeek(out var start))
        {
            return Math.Max(best, index - start);
        }

        stack.Push(index);

        return best;
    }
}
