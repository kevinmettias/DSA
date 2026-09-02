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
    public static NonAdjacentSumNode Identity => new(0, NonAdjacentSumNode.NegativeInfinity, NonAdjacentSumNode.NegativeInfinity, NonAdjacentSumNode.NegativeInfinity);

    public static NonAdjacentSumNode Combine(NonAdjacentSumNode left, NonAdjacentSumNode right)
    {
        var neither = Math.Max(
            left.Neither + Math.Max(right.Neither, right.LeftOnly),
            left.RightOnly + right.Neither);

        var leftOnly = Math.Max(
            left.LeftOnly + Math.Max(right.Neither, right.LeftOnly),
            left.Both + right.Neither);

        var rightOnly = Math.Max(
            left.Neither + Math.Max(right.RightOnly, right.Both),
            left.RightOnly + right.RightOnly);

        var both = Math.Max(
            left.LeftOnly + Math.Max(right.RightOnly, right.Both),
            left.Both + right.RightOnly);

        return new NonAdjacentSumNode(neither, leftOnly, rightOnly, both);
    }
}
