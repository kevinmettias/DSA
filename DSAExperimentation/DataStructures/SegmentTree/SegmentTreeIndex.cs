using DSAExperimentation.Algorithms;

namespace DSAExperimentation.DataStructures.SegmentTree;

// Child positions in a complete binary tree packed left-to-right into a flat array - the same
// arithmetic Collections/Heap/HeapArrayIndex.cs uses, replicated rather than reused: SegmentTree
// and Heap are different domains that happen to need the same "array as complete binary tree"
// layout (ARCHITECTURE.md §5 step 5 - replicate the pattern, not the type). BranchingFactor itself
// is shared via AlgorithmConstants rather than re-declared here, the same way HeapArrayIndex now
// does - a bare numeric constant is not a Representation/Topology contract, so sharing it isn't
// the domain-reuse §5 step 5 forbids (see AlgorithmConstants.cs's own doc comment). No Parent
// here, unlike HeapArrayIndex: SegmentTree's Build/Update/Query only ever recurse downward from
// the root, never walk back up, so there is nothing that would call it (§5 step 1 - name only the
// operations actually used).
internal static class SegmentTreeIndex
{
    public static int LeftChild(int node) => (AlgorithmConstants.BranchingFactor * node) + 1;

    public static int RightChild(int node) => LeftChild(node) + 1;
}
