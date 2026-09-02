namespace DSAExperimentation.LeetCode.FindXValueOfArrayII;

// One segment-tree node's aggregate over a range [lo, hi]: Product is the range's
// own product mod k (order-dependent, so this is a monoid element, not a group one -
// exactly what SegmentTree<Element,TOperation> is built for). Counts[r, x] is the
// number of prefixes of the range (nums[lo..j] for lo <= j <= hi) that land on
// residue x when the range is entered with an incoming multiplier of r - the
// "x-value" of every suffix-removal point in this range at once, for every possible
// starting residue simultaneously, so two nodes combine without re-walking either
// one's elements.
internal readonly struct XValueNode(int product, int[,] counts)
{
    public int Product { get; } = product;

    public int[,] Counts { get; } = counts;
}
