
namespace DSAExperimentation.DataStructures.SegmentTree;

// Groups the (Node, Start, End) triple Build/Update/Query's recursion narrows - the same "group
// related parameters into a type" recipe ARCHITECTURE.md §6 already applies to
// Sorting.SortBounds/Searching.SearchRange. Node is the array index into SegmentTreeArray<Element>;
// [Start, End] is the logical leaf range that node covers - two different things that always
// travel together through this recursion, never independently.
internal readonly record struct SegmentRange(int Node, int Start, int End)
{
    // Both SegmentTree and LazySegmentTree recurse by splitting a range into its two child
    // ranges at the midpoint - Split centralizes that shared arithmetic once, here, rather than
    // each caller re-deriving mid/left-child/right-child independently (the cross-file
    // duplication a repo-wide structural-duplication check flagged when each did).
    public (SegmentRange Left, SegmentRange Right) Split()
    {
        var mid = Start + ((End - Start) / AlgorithmConstants.HalvingFactor);

        return (
            new SegmentRange(SegmentTreeIndex.LeftChild(Node), Start, mid),
            new SegmentRange(SegmentTreeIndex.RightChild(Node), mid + 1, End));
    }
}
