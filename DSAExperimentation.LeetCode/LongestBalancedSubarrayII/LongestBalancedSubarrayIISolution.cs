using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.LeetCode.LongestBalancedSubarrayII;

// LeetCode 3721. Longest Balanced Subarray II: LongestBalancedSubarrayI's same
// question ("distinct-even-value count == distinct-odd-value count") at n <= 1e5,
// where the O(n^2) window scan that suffices for the smaller sibling is too slow.
//
// The classic "count distinct values ending at r" trick: walking r left to right,
// only the LAST occurrence (so far) of a value should count toward its parity's
// distinct total for any window ending at r - an earlier occurrence of the same
// value gets "deactivated" the moment a later one appears. Give every 1-indexed
// position k a weight (+1 odd, -1 even) that is "active" exactly while k is the
// last occurrence of nums[k-1] seen so far, and let Balance[k] be the running sum
// of active weights over positions 1..k (Balance[0] = 0, the empty prefix). Then
// for any l <= r, (distinct odds - distinct evens) in nums[l..r-1] (0-indexed,
// length r-l) equals Balance[r] - Balance[l] using activity as of r - true because
// "last occurrence up to r" is automatically "last occurrence within [l, r]" too
// for any l. A subarray is balanced exactly when that difference is 0, so the
// longest one ending at r is r minus the LEFTMOST l < r with Balance[l] ==
// Balance[r].
//
// Balance is maintained as a LazySegmentTree<BalanceRange, int,
// BalanceRangeAddOperation> of size n+1: activating position k (or deactivating a
// stale one at q) is a range-add of the weight over the suffix [k, n] (or [q, n])
// - future, not-yet-reached positions are pre-filled with a contribution that is
// only actually read once r catches up to them, by which point every relevant
// (de)activation has already landed. Finding the leftmost matching Balance[l] is
// then a binary descent driven purely by the tree's own Query(left, right) -
// BalanceRangeAddOperation's own doc comment explains why comparing a target
// against a range's [Min, Max] is enough to decide which half actually contains
// it, so no change to LazySegmentTree is needed to support the search.
internal static class LongestBalancedSubarrayIISolution
{
    // The textbook answer: reset a fresh pair of hash sets at every start index
    // and extend the window rightward - the same O(n^2) shape
    // LongestBalancedSubarrayI's brute force uses, deliberately written without
    // this repo's primitives. Correct at any n, just too slow at LC 3721's n <=
    // 1e5 to be the real answer - the arm the segment-tree strategy below has to
    // justify itself against.
    public static int FindLongestBalancedLengthByBruteForce(int[] nums)
    {
        var longest = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            var evens = new HashSet<int>();
            var odds = new HashSet<int>();

            for (var end = start; end < nums.Length; end++)
            {
                (IsEven(nums[end]) ? evens : odds).Add(nums[end]);

                if (evens.Count == odds.Count)
                {
                    longest = Math.Max(longest, end - start + 1);
                }
            }
        }

        return longest;
    }

    // One left-to-right pass: activate/deactivate the current value's contribution
    // in the balance tree, then binary-descend it for the leftmost equal-balance
    // position. Each of the n steps does O(1) range-adds plus an O(log^2 n)
    // search (O(log n) halvings of the query range, each an O(log n) tree Query),
    // for O(n log^2 n) overall.
    public static int FindLongestBalancedLengthByPrefixBalanceSegmentTree(int[] nums)
    {
        var count = nums.Length;
        var initial = new BalanceRange[count + 1];
        Array.Fill(initial, new BalanceRange(0, 0));

        var balance = new LazySegmentTree<BalanceRange, int, BalanceRangeAddOperation>(initial);
        var lastSeenPosition = new HashMap<int, int>();
        var longest = 0;

        for (var position = 1; position <= count; position++)
        {
            ApplyActivation(balance, lastSeenPosition, nums, position);

            var ending = LongestEndingAt(balance, position);
            longest = Math.Max(longest, ending);
        }

        return longest;
    }

    // Makes position the live occurrence of its value: the previous occurrence, if any,
    // stops counting toward its parity (a range-add over the suffix it still covers),
    // and position itself starts counting.
    private static void ApplyActivation(
        LazySegmentTree<BalanceRange, int, BalanceRangeAddOperation> balance,
        HashMap<int, int> lastSeenPosition,
        int[] nums,
        int position)
    {
        var value = nums[position - 1];
        var weight = IsEven(value) ? -1 : 1;

        if (lastSeenPosition.TryGetValue(value, out var previousPosition))
        {
            balance.UpdateRange(previousPosition, nums.Length, -weight);
        }

        balance.UpdateRange(position, nums.Length, weight);
        lastSeenPosition.Set(value, position);
    }

    // Balance[position] is always attained at position itself, so it always lies
    // within the [0, position - 1] prefix's own [Min, Max] whenever some earlier
    // index shares it - the precondition BinaryDescendForBalance's invariant
    // needs. When it doesn't, no balanced subarray ends here.
    private static int LongestEndingAt(
        LazySegmentTree<BalanceRange, int, BalanceRangeAddOperation> balance, int position)
    {
        var target = balance.Query(position, position).Min;
        var prefix = balance.Query(0, position - 1);

        if (target < prefix.Min || target > prefix.Max)
        {
            return 0;
        }

        return SpanToLeftmostBalance(balance, position, target);
    }

    // Reached only once the target is known to lie within the [0, position - 1] prefix's own
    // [Min, Max], so the leftmost index holding it is somewhere in that prefix and the
    // balanced run ending here reaches back exactly that far.
    private static int SpanToLeftmostBalance(
        LazySegmentTree<BalanceRange, int, BalanceRangeAddOperation> balance, int position, int target)
    {
        var matchPosition = BinaryDescendForBalance(balance, 0, position - 1, target);
        return position - matchPosition;
    }

    // Repeated halving over the index range [low, high], each half's membership
    // decided by one Query call - no access to LazySegmentTree's own internals.
    // Preferring the left half first is what makes the result leftmost.
    private static int BinaryDescendForBalance(
        LazySegmentTree<BalanceRange, int, BalanceRangeAddOperation> balance, int low, int high, int target)
    {
        while (low < high)
        {
            var mid = low + ((high - low) / 2);
            var leftHalf = balance.Query(low, mid);

            if (target >= leftHalf.Min && target <= leftHalf.Max)
            {
                high = mid;
            }
            else
            {
                low = mid + 1;
            }
        }

        return low;
    }

    // Parity is what decides which side of a balance a value counts toward.
    private static bool IsEven(int value) => value % 2 == 0;
}
