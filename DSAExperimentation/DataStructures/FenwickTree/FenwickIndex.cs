namespace DSAExperimentation.DataStructures.FenwickTree;

// The two trees a Fenwick array holds implicitly, as the step from a node to its parent in each:
// the role HeapArrayIndex plays for a heap, arithmetic over the layout rather than a stored
// relationship. Node i (1-based) holds the combination of the elements in (i - (i & -i), i].
// Adding that lowest set bit climbs the update tree to the next node whose range covers i's, so an
// update walks UpdateTreeParent until it passes the end. Taking the bit away climbs the query tree
// to the node holding the elements just before i's range, so a prefix query walks QueryTreeParent
// down to 0, the query tree's root, which holds nothing.
//
// Neither tree is a topology witness the generic tree engines could walk (ARCHITECTURE.md §13.13):
// a node is an index into the array, not an object, and which children a node has depends on the
// array's length, which a static GetChildren(node) cannot see.
//
// Each step is written out rather than through a shared lowest-set-bit helper, so a walk that
// inlines it inlines one method, not two (§19.8).
//
// Public, as HeapArrayIndex is: stateless arithmetic over a layout, reaching no tree's array or
// invariant.
/// <summary>
/// Steps between the nodes of the two trees a Fenwick array holds implicitly, by 1-based position.
/// </summary>
public static class FenwickIndex
{
    /// <summary>
    /// Finds a node's parent in the update tree, the next node an update reaches.
    /// </summary>
    /// <param name="oneBasedIndex">A position from 1 to the array's length.</param>
    /// <returns>
    /// The position of the nearest node whose range covers this one's, which may lie past the end.
    /// </returns>
    public static int UpdateTreeParent(int oneBasedIndex) =>
        oneBasedIndex + (oneBasedIndex & -oneBasedIndex);

    /// <summary>
    /// Finds a node's parent in the query tree, the next node a prefix query reads.
    /// </summary>
    /// <param name="oneBasedIndex">A position from 1 to the array's length.</param>
    /// <returns>
    /// The position of the node holding the elements just before this one's range, 0 when none are
    /// left.
    /// </returns>
    public static int QueryTreeParent(int oneBasedIndex) =>
        oneBasedIndex - (oneBasedIndex & -oneBasedIndex);
}
