using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.MinimumNumberOfSwapsToMakeTheStringBalanced;

// LeetCode 1963. Minimum Number of Swaps to Make the String Balanced: the answer is
// ceil(unmatchedClosers / 2), because one swap moves a surplus ']' to the right of a
// surplus '[' and so resolves two unmatched closers at once. Both strategies compute
// the same unmatched-closer count and differ only in how they find, for each ']', the
// nearest still-unmatched '[' to its left.
//
// Matching greedily nearest-first never changes how many closers end up unmatched,
// only how expensively "nearest" gets found - which is exactly what separates the two.
internal static class MinimumNumberOfSwapsToMakeTheStringBalancedSolution
{
    private const int ClosersFixedPerSwap = 2; // each swap resolves two unmatched closing brackets

    // Baseline: for every ']' walk backwards over the prefix looking for an unmatched
    // '[', marking it consumed. O(n^2), and deliberately plain BCL throughout - it is
    // what the nearest-unmatched-opener rule looks like written out longhand.
    public static int MinSwapsByBackwardScan(string text)
    {
        var matched = new bool[text.Length];
        var unmatchedCloseCount = 0;

        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] != ']')
            {
                continue;
            }

            if (FindNearestUnmatchedOpener(text, matched, i) is { } opener)
            {
                matched[opener] = true;
            }
            else
            {
                unmatchedCloseCount++;
            }
        }

        return (unmatchedCloseCount + 1) / ClosersFixedPerSwap;
    }

    // The nearest still-unmatched '[' strictly left of beforeIndex, or null when the
    // prefix has none left - the lookup MinSwapsByStack gets for free from LIFO order.
    private static int? FindNearestUnmatchedOpener(string text, bool[] matched, int beforeIndex)
    {
        for (var j = beforeIndex - 1; j >= 0; j--)
        {
            if (text[j] == '[' && !matched[j])
            {
                return j;
            }
        }

        return null;
    }

    // This repo's own Stack<char> tracks unmatched '[' the same way ValidParentheses
    // does, resolving the nearest-unmatched-opener lookup in O(1) per character. When a
    // ']' finds the stack empty it has no partner anywhere to its left, so a swap is
    // counted and a virtual '[' is pushed in its place, standing in for whichever later
    // ']' actually gets swapped with it - which is why the running count is already
    // ceil(unmatchedClosers / 2) rather than the raw closer count.
    public static int MinSwapsByStack(string text)
    {
        var open = new RepoCharStack();
        var swaps = 0;

        foreach (var ch in text)
        {
            if (ch == '[')
            {
                open.Push(ch);
                continue;
            }

            if (open.TryPop(out _))
            {
                continue;
            }

            swaps++;
            open.Push('[');
        }

        return swaps;
    }
}
