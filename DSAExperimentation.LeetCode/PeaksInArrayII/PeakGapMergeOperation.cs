using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.PeaksInArrayII;

// SegmentTree<Element,TOperation>'s ICombineOperation witness for LC 4017:
// merges two adjacent ranges' PeakGapNode by bridging the left range's
// rightmost peak to the right range's leftmost peak - the positions strictly
// between them form exactly one more no-peak gap, on top of whichever gaps
// each side already folded into its own InteriorGapPairs. Unlike
// NonAdjacentMergeOperation (LC 3165), PeakGapNode.None IS a true two-sided
// identity for this Combine: a side with no peak at all contributes nothing
// but its own absence, so Combine falls straight through to the other side
// regardless of which side it is - the same law SumOperation/MaxOperation's
// own Identity satisfies.
internal readonly struct PeakGapMergeOperation : ICombineOperation<PeakGapNode>
{
    public static PeakGapNode Identity => PeakGapNode.None;

    public static PeakGapNode Combine(PeakGapNode left, PeakGapNode right)
    {
        if (!left.HasPeak)
        {
            return right;
        }

        if (!right.HasPeak)
        {
            return left;
        }

        var bridgeGapPairs = PeakGapNode.PairsInGap(right.MinPeak - left.MaxPeak);

        return new PeakGapNode(true, left.MinPeak, right.MaxPeak, left.InteriorGapPairs + right.InteriorGapPairs + bridgeGapPairs);
    }
}
