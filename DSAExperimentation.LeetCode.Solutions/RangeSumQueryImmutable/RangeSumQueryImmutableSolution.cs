using DSAExperimentation.Domain.Mathematics.Algebra;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.PrefixSums;

namespace DSAExperimentation.LeetCode.Solutions.RangeSumQueryImmutable;

// LeetCode 303. Range Sum Query - Immutable: NumArray(nums) is built once, then
// sumRange(left, right) may be called any number of times against it.
//
// BruteForceRescan is the textbook answer - sum nums[left..right] directly on every
// call, O(n) each time. FenwickTree is this repo's own FenwickTree<int, Addition<int>>
// (the point-update/range-query family ARCHITECTURE.md's FenwickTree worked example
// describes): built once from the array in O(n log n), so every SumRange afterward is
// an O(log n) Query instead of a rescan. PrefixSums is this repo's
// PrefixSums<int, Addition<int>>: built in O(n), every SumRange afterward is one O(1)
// subtraction, the answer for an array that never changes. FenwickTree pays its log
// factor for updates this problem never makes (LC 307 does).
//
// The strategies are the CreateBy factories: each takes the array and makes LeetCode's
// NumArray, the object the catalog harness replays LeetCode's call scripts on
// (ARCHITECTURE 17.13). Each strategy's query - one sumRange against what its
// constructor prepared - is SumRange on the nested class named for the strategy
// (BruteForceRescan, FenwickTree, PrefixSums): the object asks it and the hand-written
// benchmark calls it directly, so each strategy is written once.
[LeetCodeQuestion("range-sum-query-immutable", LeetCodeAnswerOrder.AsWritten)]
internal static class RangeSumQueryImmutableSolution
{
    // LeetCode's shape: a NumArray that keeps the array as given and sums the slice
    // on every call.
    public static INumArray CreateByBruteForceRescan(int[] nums) => new BruteForceRescanArray(nums);

    // LeetCode's shape: a NumArray whose constructor builds the tree once.
    public static INumArray CreateByFenwickTree(int[] nums) => new FenwickTreeArray(nums);

    // LeetCode's shape: a NumArray whose constructor builds the running totals once.
    public static INumArray CreateByPrefixSums(int[] nums) => new PrefixSumsArray(nums);

    // The textbook answer: sum the slice directly every call. Deliberately
    // written without this repo's primitives - it is the arm the tree strategy
    // below has to justify itself against.
    internal static class BruteForceRescan
    {
        public static int SumRange(int[] nums, int left, int right)
        {
            var total = 0;

            for (var i = left; i <= right; i++)
            {
                total += nums[i];
            }

            return total;
        }
    }

    // The prepared-input query: one query against the tree NumArray's constructor
    // built, so a caller answering many queries against the same array pays
    // construction once rather than per query.
    internal static class FenwickTree
    {
        public static int SumRange(FenwickTree<int, Addition<int>> tree, int left, int right) =>
            tree.Query(left, right);
    }

    // The prepared-input query: one query against the totals NumArray's constructor
    // built. LeetCode's bounds keep every total within 10^4 * 10^5, inside an int.
    internal static class PrefixSums
    {
        public static int SumRange(PrefixSums<int, Addition<int>> sums, int left, int right) =>
            sums.Query(left, right);
    }

    // The array as given; every query sums its window.
    private sealed class BruteForceRescanArray(int[] nums) : INumArray
    {
        public int SumRange(int left, int right) => BruteForceRescan.SumRange(nums, left, right);
    }

    // A Fenwick tree over the array, built once; every query is an O(log n) Query.
    private sealed class FenwickTreeArray(int[] nums) : INumArray
    {
        private readonly FenwickTree<int, Addition<int>> _tree = new(nums);

        public int SumRange(int left, int right) => FenwickTree.SumRange(_tree, left, right);
    }

    // The array's running totals, built once; every query is one subtraction.
    private sealed class PrefixSumsArray(int[] nums) : INumArray
    {
        private readonly PrefixSums<int, Addition<int>> _sums = new(nums);

        public int SumRange(int left, int right) => PrefixSums.SumRange(_sums, left, right);
    }
}
