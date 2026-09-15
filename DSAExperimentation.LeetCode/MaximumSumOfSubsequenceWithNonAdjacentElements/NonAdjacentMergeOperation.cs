using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.MaximumSumOfSubsequenceWithNonAdjacentElements;

// SegmentTree<Element,TOperation>'s ICombineOperation witness for LC 3165: merges
// two adjacent segments' NonAdjacentSumNode by marginalizing over whether the
// left segment's own rightmost element was picked - if it was, the right
// segment's leftmost element cannot also be picked (they are adjacent in the
// combined range), so its dp[0][*] answers are the only ones eligible; if it
// wasn't, both of the right segment's own answers remain candidates.
//
// Identity is unreachable for this problem's own access pattern: every query
// this solution issues is the tree's full [0, Count-1] range, which
// SegmentTree.Query resolves as one direct node lookup at the root with no
// partial-range recursion, the only place Identity is ever read. It is set to
// the "nothing picked, nothing pickable" reading of an empty range anyway,
// for interface completeness - though note Combine's own merge formula
// assumes both sides are non-empty real segments (it marginalizes over
// whether a real rightmost/leftmost boundary element was picked), so unlike
// MaxOperation/SumOperation this Combine is not a true two-sided identity law
// for arbitrary x; it is simply never asked to be one here.
internal readonly struct NonAdjacentMergeOperation : ICombineOperation<NonAdjacentSumNode>
{
    public static NonAdjacentSumNode Identity => new(
        0,
        NonAdjacentSumBound.NegativeInfinity,
        NonAdjacentSumBound.NegativeInfinity,
        NonAdjacentSumBound.NegativeInfinity);

    public static NonAdjacentSumNode Combine(NonAdjacentSumNode left, NonAdjacentSumNode right)
    {
        var neither = BestForBoundary(
            left.Neither, left.RightOnly, right.Neither, right.LeftOnly);

        var leftOnly = BestForBoundary(
            left.LeftOnly, left.Both, right.Neither, right.LeftOnly);

        var rightOnly = BestForBoundary(
            left.Neither, left.RightOnly, right.RightOnly, right.Both);

        var both = BestForBoundary(
            left.LeftOnly, left.Both, right.RightOnly, right.Both);

        return new NonAdjacentSumNode(neither, leftOnly, rightOnly, both);
    }

    // One row of the merge's boundary table. The row already fixes whether each
    // segment's own outer element is picked, so each side contributes the two of
    // its totals that agree with it: the one whose join-adjacent element is left
    // unpicked (`...Open`) and the one where that element is picked (`...Taken`).
    // The two halves of the join are adjacent, so they can never both be picked -
    // hence a taken left join end leaves only the right segment's open total.
    private static long BestForBoundary(long leftOpen, long leftTaken, long rightOpen, long rightTaken) =>
        Math.Max(leftOpen + Math.Max(rightOpen, rightTaken), leftTaken + rightOpen);
}
