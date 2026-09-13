using OpenerStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.MaximumNestingDepthOfTheParentheses;

// LeetCode 1614. Maximum Nesting Depth of the Parentheses: report the deepest
// nesting level reached by a valid parentheses expression. Digits and the four
// arithmetic operators are irrelevant to the answer and pass through untouched.
//
// Both strategies are one left-to-right pass reporting the running depth's high
// water mark. They differ only in what carries that depth: a plain counter, or this
// repo's own Stack<char> holding each unmatched opener explicitly so its own Count
// is the depth - the same "stack size as depth" trick RemoveOutermostParentheses
// uses to decide emission, here reporting the maximum instead.
internal static class MaximumNestingDepthOfTheParenthesesSolution
{
    private const char Opener = '(';

    private const char Closer = ')';

    // Nothing is open, so an expression without parentheses answers with this.
    private const int NoNesting = 0;

    // The textbook answer: one int tracks how deeply nested the scan currently is
    // and no unmatched opener is ever stored. Deliberately written with nothing but
    // the BCL - it is the arm MaxDepthByOpenerStack has to justify itself against.
    public static int MaxDepthByRunningCounter(string expression)
    {
        var depth = NoNesting;
        var maxDepth = NoNesting;

        foreach (var symbol in expression)
        {
            if (symbol == Opener)
            {
                depth++;
                maxDepth = Math.Max(maxDepth, depth);
            }
            else if (symbol == Closer)
            {
                depth--;
            }
        }

        return maxDepth;
    }

    // This repo's own Stack<char> - the LIFO primitive ValidParentheses and
    // RemoveOutermostParentheses already use - holds each unmatched opener, so its
    // Count stands in for the running depth directly.
    public static int MaxDepthByOpenerStack(string expression)
    {
        var openers = new OpenerStack();
        var maxDepth = NoNesting;

        foreach (var symbol in expression)
        {
            if (symbol == Opener)
            {
                openers.Push(symbol);
                maxDepth = Math.Max(maxDepth, openers.Count);
            }
            else if (symbol == Closer)
            {
                openers.TryPop(out _);
            }
        }

        return maxDepth;
    }
}
