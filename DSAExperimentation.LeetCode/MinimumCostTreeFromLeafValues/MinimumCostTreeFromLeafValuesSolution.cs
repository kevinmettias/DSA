using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.MinimumCostTreeFromLeafValues;

// LeetCode 1130. Minimum Cost Tree From Leaf Values: arr is the in-order sequence
// of leaves of a binary tree whose every internal node holds the product of the
// largest leaf in its left subtree and the largest leaf in its right subtree.
// Return the smallest possible sum of those internal-node values.
//
// MctFromLeafValuesByUnmemoizedRecursion is the textbook interval recursion over
// (left, right) leaf-index bounds, recomputing max(arr[left..split]) and
// max(arr[split+1..right]) freshly for every split candidate - exponential, since
// each sub-range is re-explored through every enclosing split. It is the arm the
// composed strategy has to justify itself against.
//
// MctFromLeafValuesByMonotonicStack skips the DP entirely. A leaf that is smaller
// than both its still-open neighbours can only ever be optimal to combine with the
// SMALLER of them - whichever neighbour it is merged into, that neighbour's maximum
// is what every later merge sees, so pairing it with the smaller one costs least and
// changes nothing afterwards. So one monotonic-decreasing pass over this repo's own
// Stack<int> (the SumOfSubarrayMinimums precedent) suffices: pop a leaf as soon as
// the next one is >= it and charge mid * min(newTop, current). A sentinel of
// int.MaxValue at the bottom of the stack stands in for "no neighbour on this side
// yet", so it never wins a Math.Min and never gets popped. O(n).
internal static class MinimumCostTreeFromLeafValuesSolution
{
    // The sentinel plus one real leaf: below this nothing is left to merge.
    private const int RemainingStackFloor = 2;

    // The textbook answer: every split of every leaf range, with no memoization
    // and no repo primitives - deliberately what you would write first.
    public static int MctFromLeafValuesByUnmemoizedRecursion(int[] arr)
        => (int)MinCost(arr, 0, arr.Length - 1);

    public static int MctFromLeafValuesByMonotonicStack(int[] arr)
    {
        var stack = new RepoIntStack();
        stack.Push(int.MaxValue);

        var total = MergeSmallerNeighbors(stack, arr) + DrainRemaining(stack);

        return (int)total;
    }

    private static long MinCost(int[] arr, int left, int right)
    {
        if (left == right)
        {
            return 0;
        }

        var best = long.MaxValue;

        for (var split = left; split < right; split++)
        {
            var cost = MinCost(arr, left, split) + MinCost(arr, split + 1, right)
                + ((long)MaxIn(arr, left, split) * MaxIn(arr, split + 1, right));
            best = Math.Min(best, cost);
        }

        return best;
    }

    private static int MaxIn(int[] arr, int left, int right)
    {
        var max = arr[left];

        for (var i = left + 1; i <= right; i++)
        {
            max = Math.Max(max, arr[i]);
        }

        return max;
    }

    // Every leaf the newcomer dominates is merged away now, against whichever of
    // its two neighbours is smaller.
    private static long MergeSmallerNeighbors(RepoIntStack stack, int[] arr)
    {
        long total = 0;

        foreach (var value in arr)
        {
            while (stack.TryPeek(out var top) && top <= value)
            {
                stack.TryPop(out var mid);
                stack.TryPeek(out var next);
                total += (long)mid * Math.Min(next, value);
            }

            stack.Push(value);
        }

        return total;
    }

    // What survives the sweep is a strictly decreasing run, which can only be
    // folded from the bottom up.
    private static long DrainRemaining(RepoIntStack stack)
    {
        long total = 0;

        while (stack.Count > RemainingStackFloor)
        {
            stack.TryPop(out var mid);
            stack.TryPeek(out var next);
            total += (long)mid * next;
        }

        return total;
    }
}
