using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.LeetCode.RangeSumQueryImmutable;

// LeetCode 303. Range Sum Query - Immutable: NumArray(nums) is built once, then
// sumRange(left, right) may be called any number of times against it.
//
// SumRangeByBruteForceRescan is the textbook answer - sum nums[left..right]
// directly on every call, O(n) each time. SumRangeByFenwickTree is this repo's
// own FenwickTree<int, SumOperation<int>> (the point-update/range-query family
// ARCHITECTURE.md's FenwickTree worked example describes): built once from the
// array in O(n log n), so every SumRange afterward is an O(log n) Query instead
// of a rescan.
internal static class RangeSumQueryImmutableSolution
{
    // The textbook answer: sum the slice directly every call. Deliberately
    // written without this repo's primitives - it is the arm the tree strategy
    // below has to justify itself against.
    public static int SumRangeByBruteForceRescan(int[] nums, int left, int right)
    {
        var total = 0;

        for (var i = left; i <= right; i++)
        {
            total += nums[i];
        }

        return total;
    }

    // LeetCode's shape: builds NumArray's tree fresh for this one call.
    public static int SumRangeByFenwickTree(int[] nums, int left, int right) =>
        SumRangeByFenwickTree(new FenwickTree<int, SumOperation<int>>(nums), left, right);

    // The prepared-input overload: the tree NumArray's constructor would already
    // have built, so a caller answering many queries against the same array
    // pays construction once rather than per query.
    public static int SumRangeByFenwickTree(FenwickTree<int, SumOperation<int>> tree, int left, int right) =>
        tree.Query(left, right);
}
