
namespace DSAExperimentation.DataStructures.Heap;

// Parent/child positions in a complete binary tree packed left-to-right into a flat array -
// the same role GridChildren's (dRow, dCol) arithmetic plays for a grid: a derivation from the
// chosen physical layout, not a stored relationship.
//
// Public while HeapArray stays internal: this is stateless arithmetic over a layout, so a caller
// walking any heap-numbered tree (LC 2509's node ids) can share it without reaching the heap's
// storage or any invariant Heap keeps.
/// <summary>
/// Positions of a node's parent and children in a complete binary tree stored level by level in a flat array.
/// </summary>
public static class HeapArrayIndex
{
    /// <summary>Finds where a node's parent sits.</summary>
    /// <param name="index">The 0-based position of any node except the root.</param>
    /// <returns>The 0-based position of that node's parent.</returns>
    public static int Parent(int index) => (index - 1) / AlgorithmConstants.BranchingFactor;

    /// <summary>Finds where a node's left child sits, whether or not the array holds one there.</summary>
    /// <param name="index">The 0-based position of the node.</param>
    /// <returns>The 0-based position of that node's left child.</returns>
    public static int LeftChild(int index) => (AlgorithmConstants.BranchingFactor * index) + 1;

    /// <summary>Finds where a node's right child sits, whether or not the array holds one there.</summary>
    /// <param name="index">The 0-based position of the node.</param>
    /// <returns>The 0-based position of that node's right child.</returns>
    public static int RightChild(int index) => LeftChild(index) + 1;
}
