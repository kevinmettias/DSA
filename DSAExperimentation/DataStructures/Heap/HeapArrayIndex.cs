namespace DSAExperimentation.DataStructures.Heap;

// Parent/child positions in a complete binary tree packed left-to-right into a flat array -
// the same role GridChildren's (dRow, dCol) arithmetic plays for a grid: a derivation from the
// chosen physical layout, not a stored relationship.
internal static class HeapArrayIndex
{
    private const int BranchingFactor = 2;

    public static int Parent(int index) => (index - 1) / BranchingFactor;

    public static int LeftChild(int index) => (BranchingFactor * index) + 1;

    public static int RightChild(int index) => LeftChild(index) + 1;
}
