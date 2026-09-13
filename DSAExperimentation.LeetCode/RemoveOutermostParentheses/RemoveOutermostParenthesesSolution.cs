using System.Text;
using OpenerStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.RemoveOutermostParentheses;

// LeetCode 1021. Remove Outermost Parentheses: split a valid parentheses string
// into its primitive pieces - each a run back down to depth 0 - and concatenate
// every piece with its own outermost pair removed.
//
// Both strategies are one left-to-right pass appending everything except the
// opener seen at depth 0 and the closer that returns to depth 0. They differ only
// in what carries that depth: a plain running counter, or this repo's own
// Stack<char> holding each unmatched opener explicitly so its Count is the depth.
internal static class RemoveOutermostParenthesesSolution
{
    private const char Opener = '(';

    // Nothing is open yet, so the next opener is a primitive's outermost one.
    private const int OutermostDepth = 0;

    // The textbook answer: one int tracks how deeply nested the scan currently is,
    // no unmatched opener is ever stored. Deliberately written with nothing but the
    // BCL - it is the arm RemoveOuterParenthesesByOpenerStack has to justify itself
    // against.
    public static string RemoveOuterParenthesesByDepthCounter(string expression)
    {
        var result = new StringBuilder(expression.Length);
        var depth = OutermostDepth;

        foreach (var symbol in expression)
        {
            depth = AppendIfInner(result, symbol, depth);
        }

        return result.ToString();
    }

    // An opener is emitted only when something is already open, and a closer is
    // counted down before the same test, so each primitive's outermost pair is the
    // one pair excluded.
    private static int AppendIfInner(StringBuilder result, char symbol, int depth)
    {
        if (symbol == Opener)
        {
            if (depth > OutermostDepth)
            {
                result.Append(symbol);
            }

            return depth + 1;
        }

        depth--;

        if (depth > OutermostDepth)
        {
            result.Append(symbol);
        }

        return depth;
    }

    // This repo's own Stack<char> - the LIFO primitive ValidParentheses and
    // MinimumAddToMakeParenthesesValid already use - holds each unmatched opener, so
    // its own Count doubles as the running depth: the "stack size as depth" trick
    // ScoreOfParentheses uses, here deciding emission rather than scoring.
    public static string RemoveOuterParenthesesByOpenerStack(string expression)
    {
        var openers = new OpenerStack();
        var result = new StringBuilder(expression.Length);

        foreach (var symbol in expression)
        {
            AppendIfNested(openers, result, symbol);
        }

        return result.ToString();
    }

    private static void AppendIfNested(OpenerStack openers, StringBuilder result, char symbol)
    {
        if (symbol == Opener)
        {
            if (openers.Count > OutermostDepth)
            {
                result.Append(symbol);
            }

            openers.Push(symbol);
            return;
        }

        // Pops first so the post-pop Count symmetrically excludes the outermost closer.
        openers.TryPop(out _);

        if (openers.Count > OutermostDepth)
        {
            result.Append(symbol);
        }
    }
}
