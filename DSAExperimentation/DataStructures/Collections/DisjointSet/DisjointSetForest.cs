namespace DSAExperimentation.DataStructures.Collections.DisjointSet;

// Concrete, not behind an interface - there is exactly one disjoint-set representation
// today. See ARCHITECTURE.md §5: an interface here would be the same speculative
// abstraction IChildren's own doc comment warns against, since no second implementation
// exists yet to justify one.
//
// Fixed-size at construction, unlike every other Collections/** representation
// (HeapArray/DynamicArray/HashMap all start empty and grow): a disjoint-set forest's
// universe of ids is conventionally known upfront, so there is no Add/growth path here
// by design, not oversight. Ids are dense integers in [0, Count) - out-of-range ids
// throw, but nothing here verifies a caller assigned ids consistently.
//
// GetParent/SetParent/GetRank/IncrementRank are O(1) by construction (raw int[]
// indexers) - DisjointSet's O(a(n)) amortized Find/Union claim depends on this. See
// ARCHITECTURE.md §8: swapping this backing store for anything with O(n) indexed
// access would silently degrade that claim with no compiler error.
//
// Rank is persisted state, not recomputed - unlike Heap's IHeapOrder.HasPriority, which
// recomputes fresh on every call with nothing cached, reconstructing "which subtree is
// taller" from parent alone would require an O(n) scan, defeating the point of the
// heuristic. IncrementRank, not a general SetRank: DisjointSet.Union's only rank
// mutation is "+1 on the new root, on a tie," so that's the whole surface it needs.
internal sealed class DisjointSetForest
{
    private const string InvalidIdMessage = "Id was outside the bounds of the disjoint-set universe.";

    private readonly int[] _parent;
    private readonly int[] _rank;

    public DisjointSetForest(int count)
    {
        _parent = new int[count];
        _rank = new int[count];

        for (var id = 0; id < count; id++)
        {
            _parent[id] = id;
        }
    }

    public int Count => _parent.Length;

    public int GetParent(int id)
    {
        ValidateId(id);
        return _parent[id];
    }

    public void SetParent(int id, int parent)
    {
        ValidateId(id);
        _parent[id] = parent;
    }

    public int GetRank(int id)
    {
        ValidateId(id);
        return _rank[id];
    }

    public void IncrementRank(int id)
    {
        ValidateId(id);
        _rank[id]++;
    }

    private void ValidateId(int id)
    {
        if (id < 0 || id >= Count)
        {
            throw new ArgumentOutOfRangeException(nameof(id), InvalidIdMessage);
        }
    }
}
