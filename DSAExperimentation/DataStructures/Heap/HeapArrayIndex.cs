
namespace DSAExperimentation.DataStructures.Heap;

// Parent/child positions in a complete binary tree packed left-to-right into a flat array -
// the same role GridChildren's (dRow, dCol) arithmetic plays for a grid: a derivation from the
// chosen physical layout, not a stored relationship.
//
// Public while HeapArray stays internal: this is stateless arithmetic over a layout, so a caller
// walking any heap-numbered tree (LC 2509's node ids) can share it without reaching the heap's
// storage or any invariant Heap keeps.
public static class HeapArrayIndex
{
    public static int Parent(int index) => (index - 1) / AlgorithmConstants.BranchingFactor;

    public static int LeftChild(int index) => (AlgorithmConstants.BranchingFactor * index) + 1;

    public static int RightChild(int index) => LeftChild(index) + 1;
}
