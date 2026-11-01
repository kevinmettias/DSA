
namespace DSAExperimentation.DataStructures.DisjointSet;

// Find/Union run in O(a(n)) amortized time (inverse Ackermann, effectively constant for
// any n representable here) on a forest Union alone has joined, because path compression and
// union-by-rank are then both applied to every link. Either alone still degrades to O(log n)
// amortized - which is where a forest MergeInto has linked lands, since MergeInto never
// consults rank - and naive linking with no path compression degrades worst case to O(n). See
// ARCHITECTURE.md §5's Complexity-law row and DisjointSetForest.cs for the O(1)
// indexed-access dependency this claim also relies on.
//
// Unlike Heap<T,TOrder>'s IHeapOrder, there is no injected linking-policy witness here:
// union-by-rank vs. union-by-size vs. naive linking never changes which partition
// Find/IsConnected report for a given sequence of calls - only tree height, and therefore
// amortized cost, degrades silently with no compiler error and no failing correctness
// test to catch a worse policy. Rank-guided linking is hardcoded as the standard
// combination rather than a swappable strategy for that reason - and Union's contract never
// names the representative; MergeInto's does.
//
// Ids are dense integers in [0, Count) assigned by the caller before any Find/Union
// call - an unchecked-by-this-type precondition the same shape as BinarySearch's
// sortedness precondition (out-of-range ids throw via DisjointSetForest's own bounds
// check, but nothing here verifies ids were assigned consistently by the caller).
/// <summary>
/// A partition of the ids 0 to Count - 1 into disjoint sets, each id starting alone, that can be merged and queried.
/// </summary>
public sealed class DisjointSet
{
    private readonly DisjointSetForest _forest;

    /// <summary>The number of ids, fixed at construction; not the number of sets.</summary>
    public int Count => _forest.Count;

    public DisjointSet(int count) => _forest = new DisjointSetForest(count);

    /// <summary>Finds the representative of an id's set, flattening the path to it on the way.</summary>
    /// <param name="id">An id from 0 to <see cref="Count"/> - 1.</param>
    /// <returns>The representative's id, shared by every id in the set until a later merge changes the set.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The id is negative or not below <see cref="Count"/>.</exception>
    public int Find(int id)
    {
        var root = id;

        while (_forest.GetParent(root) != root)
        {
            root = _forest.GetParent(root);
        }

        CompressPath(id, root);
        return root;
    }

    private void CompressPath(int id, int root)
    {
        while (_forest.GetParent(id) != root)
        {
            var next = _forest.GetParent(id);
            _forest.SetParent(id, root);
            id = next;
        }
    }

    /// <summary>Merges two ids' sets, leaving unspecified which of the two representatives survives.</summary>
    /// <param name="first">An id in one of the sets to merge.</param>
    /// <param name="second">An id in the other set; nothing merges when both ids already share a set.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Either id is negative or not below <see cref="Count"/>.
    /// </exception>
    public void Union(int first, int second)
    {
        var firstRoot = Find(first);
        var secondRoot = Find(second);

        if (firstRoot != secondRoot)
        {
            LinkByRank(firstRoot, secondRoot);
        }
    }

    private void LinkByRank(int firstRoot, int secondRoot)
    {
        var firstRank = _forest.GetRank(firstRoot);
        var secondRank = _forest.GetRank(secondRoot);

        if (firstRank < secondRank)
        {
            _forest.SetParent(firstRoot, secondRoot);
        }
        else if (firstRank > secondRank)
        {
            _forest.SetParent(secondRoot, firstRoot);
        }
        else
        {
            _forest.SetParent(secondRoot, firstRoot);
            _forest.IncrementRank(firstRoot);
        }
    }

    // Joins id's set into target's and promises which root survives: the one Find(target)
    // returned before the call, until a later Union or MergeInto touches either set. Union leaves
    // that root unspecified, which is what keeps its linking policy a Complexity law (ARCHITECTURE.md
    // §10.1); this names it, so it is a separate operation, not a policy. A caller that reads the
    // representative as an answer - "the nearest slot still free at or before x" - links one way
    // with this and reads Find. It links without consulting rank, so the O(a(n)) claim above holds
    // only for a forest Union alone has joined; once the two mix, path compression alone bounds
    // Find at O(log n) amortized.
    /// <summary>Merges an id's set into a target id's set, whose representative the merged set keeps.</summary>
    /// <param name="id">An id in the set to merge away.</param>
    /// <param name="target">An id in the set to merge into; nothing merges when both ids already share a set.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Either id is negative or not below <see cref="Count"/>.
    /// </exception>
    public void MergeInto(int id, int target)
    {
        var idRoot = Find(id);
        var targetRoot = Find(target);

        if (idRoot != targetRoot)
        {
            _forest.SetParent(idRoot, targetRoot);
        }
    }

    /// <summary>Reports whether two ids are in the same set.</summary>
    /// <param name="first">One id to compare.</param>
    /// <param name="second">The other id to compare.</param>
    /// <returns>True when both ids share a representative, as any id does with itself; false otherwise.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Either id is negative or not below <see cref="Count"/>.
    /// </exception>
    public bool IsConnected(int first, int second) => Find(first) == Find(second);
}
