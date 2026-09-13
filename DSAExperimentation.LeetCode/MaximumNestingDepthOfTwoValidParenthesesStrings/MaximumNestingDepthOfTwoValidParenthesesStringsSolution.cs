using OpenerStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.MaximumNestingDepthOfTwoValidParenthesesStrings;

// LeetCode 1111. Maximum Nesting Depth of Two Valid Parentheses Strings: split a
// valid parentheses string into two subsequences whose larger nesting depth is as
// small as possible, reporting which group each character joins.
//
// Splitting by the parity of a character's nesting depth is optimal: consecutive
// depths alternate groups, so each subsequence keeps only every other level and
// the original max depth is halved. Both strategies below answer with that same
// parity - they differ only in how the depth at each position is obtained.
internal static class MaximumNestingDepthOfTwoValidParenthesesStringsSolution
{
    // Depth parity assigns each character to one of two groups.
    private const int GroupCount = 2;

    // The textbook answer: for each position, rescan everything before it to find
    // the depth there. O(n^2) and deliberately written without this repo's
    // primitives - it is the arm the composed solution has to beat.
    public static int[] MaxDepthAfterSplitByDepthRescan(string seq)
    {
        var groups = new int[seq.Length];

        for (var i = 0; i < seq.Length; i++)
        {
            var depthBefore = 0;

            for (var j = 0; j < i; j++)
            {
                depthBefore += seq[j] == '(' ? 1 : -1;
            }

            groups[i] = seq[i] == '(' ? (depthBefore + 1) % GroupCount : depthBefore % GroupCount;
        }

        return groups;
    }

    // The composed answer: this repo's Stack<char> holds the currently-open
    // brackets, so its Count is the running depth and one left-to-right pass
    // suffices. An opener is at the depth it creates; a closer at the depth it
    // ends, which is the stack's depth just before the pop.
    public static int[] MaxDepthAfterSplitByOpenerStack(string seq)
    {
        var groups = new int[seq.Length];
        var openers = new OpenerStack();

        for (var i = 0; i < seq.Length; i++)
        {
            if (seq[i] == '(')
            {
                openers.Push(seq[i]);
                groups[i] = openers.Count % GroupCount;
            }
            else
            {
                groups[i] = openers.Count % GroupCount;
                openers.TryPop(out _);
            }
        }

        return groups;
    }
}
