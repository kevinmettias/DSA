namespace DSAExperimentation.Collections.DisjointSet;

// Find/Union run in O(a(n)) amortized time (inverse Ackermann, effectively constant for
// any n representable here) because path compression and union-by-rank are both applied
// unconditionally inside Find/Union. Either alone still degrades to O(log n) amortized;
// naive linking with no path compression degrades worst case to O(n). See
// ARCHITECTURE.md §5's Complexity-law row and DisjointSetForest.cs for the O(1)
// indexed-access dependency this claim also relies on.
//
// Unlike Heap<T,TOrder>'s IHeapOrder, there is no injected linking-policy witness here:
// union-by-rank vs. union-by-size vs. naive linking never changes which partition
// Find/Connected report for a given sequence of calls - only tree height, and therefore
// amortized cost, degrades silently with no compiler error and no failing correctness
// test to catch a worse policy. Rank-guided linking is hardcoded as the standard
// combination rather than a swappable strategy for that reason.
//
// Ids are dense integers in [0, Count) assigned by the caller before any Find/Union
// call - an unchecked-by-this-type precondition the same shape as BinarySearch's
// sortedness precondition (out-of-range ids throw via DisjointSetForest's own bounds
// check, but nothing here verifies ids were assigned consistently by the caller).
internal sealed class DisjointSet
{
    private readonly DisjointSetForest _forest;

    public DisjointSet(int count) => _forest = new DisjointSetForest(count);

    public int Count => _forest.Count;

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

    public void Union(int first, int second)
    {
        var firstRoot = Find(first);
        var secondRoot = Find(second);

        if (firstRoot != secondRoot)
        {
            LinkByRank(firstRoot, secondRoot);
        }
    }

    public bool Connected(int first, int second) => Find(first) == Find(second);

    private void CompressPath(int id, int root)
    {
        while (_forest.GetParent(id) != root)
        {
            var next = _forest.GetParent(id);
            _forest.SetParent(id, root);
            id = next;
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
}
