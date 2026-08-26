namespace DSAExperimentation.DataStructures.SegmentTree;

// Concrete, not behind an interface - exactly one segment-tree representation exists today
// (ARCHITECTURE.md §5 step 3).
//
// Fixed-size at construction (4 * leafCount, the standard recursive-segment-tree upper bound that
// covers any non-power-of-two leaf count) - like DisjointSetForest, the universe size is known
// upfront, so there is no Add/growth path here by design, not oversight.
//
// Get/Set are O(1) by construction (raw Element[] indexer) - SegmentTree<Element,TOperation>'s O(log n)
// Update/Query claim depends on this. See ARCHITECTURE.md §8. Node indices are internal-only
// (never caller-facing), so unlike DisjointSetForest's ids this doesn't need its own bounds-check
// message - an out-of-range node index would be this file's own bug, and the runtime's
// IndexOutOfRangeException already says so.
internal sealed class SegmentTreeArray<Element>
{
    private const int SizeMultiplier = 4;

    private readonly Element[] _nodes;

    public SegmentTreeArray(int leafCount) => _nodes = new Element[SizeMultiplier * Math.Max(leafCount, 1)];

    public Element Get(int node) => _nodes[node];

    public void Set(int node, Element value) => _nodes[node] = value;
}
