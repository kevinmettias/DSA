namespace DSAExperimentation.DataStructures.FenwickTree;

// Concrete, not behind an interface - exactly one Fenwick representation exists today
// (ARCHITECTURE.md §5 step 3).
//
// Fixed-size at construction (size + 1, the classic 1-indexed Fenwick convention - slot 0 is
// unused so every stored position's lowest-set-bit arithmetic stays well-defined). Get/Set take
// that same 1-indexed position FenwickTree's own bit arithmetic already works in, rather than
// hiding the offset here - FenwickTree.cs is this type's only caller and owns the 0-indexed public
// surface callers actually see.
//
// Get/Set are O(1) by construction (raw Element[] indexer) - FenwickTree<Element,TOperation>'s O(log n)
// Add/PrefixQuery claim depends on this. See ARCHITECTURE.md §8.
internal sealed class FenwickArray<Element>
{
    private readonly Element[] _nodes;

    public FenwickArray(int size) => _nodes = new Element[size + 1];

    public Element Get(int oneBasedIndex) => _nodes[oneBasedIndex];

    public void Set(int oneBasedIndex, Element value) => _nodes[oneBasedIndex] = value;
}
