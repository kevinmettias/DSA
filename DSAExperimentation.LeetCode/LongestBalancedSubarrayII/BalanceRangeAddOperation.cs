using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.LeetCode.LongestBalancedSubarrayII;

// Because the running balance changes by at most 1 between adjacent index
// positions (each position contributes -1, 0 or +1), a range's [Min, Max] is not
// just a bound - by the discrete intermediate value property every integer
// between them is actually attained somewhere in that range. That is what lets
// LongestBalancedSubarrayIISolution binary-descend the tree from outside (via
// repeated Query calls alone) to find the leftmost index holding a target value,
// with no change to LazySegmentTree itself.
internal readonly struct BalanceRangeAddOperation : IRangeUpdateOperation<BalanceRange, int>
{
    public static BalanceRange Identity => new(int.MaxValue, int.MinValue);

    public static int NoUpdate => 0;

    public static BalanceRange Combine(BalanceRange left, BalanceRange right) =>
        new(Math.Min(left.Min, right.Min), Math.Max(left.Max, right.Max));

    public static int ComposeUpdate(int outer, int inner) => outer + inner;

    // rangeLength is irrelevant to a min/max bound the way it would matter for a
    // range-sum - both endpoints simply shift by the pending delta.
    public static BalanceRange ApplyUpdate(BalanceRange aggregate, int update, int rangeLength) =>
        new(aggregate.Min + update, aggregate.Max + update);
}
