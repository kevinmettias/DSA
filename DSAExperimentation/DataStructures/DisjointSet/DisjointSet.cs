
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
internal sealed class DisjointSet
{
    private readonly DisjointSetForest _forest;

    public int Count => _forest.Count;

    public DisjointSet(int count) => _forest = new DisjointSetForest(count);

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
    public void MergeInto(int id, int target)
    {
        var idRoot = Find(id);
        var targetRoot = Find(target);

        if (idRoot != targetRoot)
        {
            _forest.SetParent(idRoot, targetRoot);
        }
    }

    public bool IsConnected(int first, int second) => Find(first) == Find(second);
}
