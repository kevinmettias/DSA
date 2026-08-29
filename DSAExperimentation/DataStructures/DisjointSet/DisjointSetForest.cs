namespace DSAExperimentation.DataStructures.DisjointSet;

// Concrete, not behind an interface - there is exactly one disjoint-set representation
// today. See ARCHITECTURE.md §5: an interface here would be the same speculative
// abstraction IChildren's own doc comment warns against, since no second implementation
// exists yet to justify one.
//
// Fixed-size at construction, unlike every other Collections/** representation
// (HeapArray/DynamicArray/HashMap all start empty and grow): a disjoint-set forest's
// universe of ids is conventionally known upfront, so there is no Add/growth path here
// by design, not oversight. Ids are dense integers in [0, Count) - out-of-range ids
// throw via the Get*/Set*/IncrementRank convenience methods, or return false via their
// TryX siblings, but nothing here verifies a caller assigned ids consistently.
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

    public int Count => _parent.Length;

    public DisjointSetForest(int count)
    {
        _parent = new int[count];
        _rank = new int[count];

        for (var id = 0; id < count; id++)
        {
            _parent[id] = id;
        }
    }

    // Throwing convenience beside each TryX's recoverable form. Unlike Heap/Stack/
    // Deque/Queue/HashMap - whose Peek/Pop/Get once had a throwing form too, since
    // removed - this one stays: an out-of-range id is DisjointSet.cs's own
    // internal-trusted caller's bug, not state an external caller can't already know,
    // so throwing loudly here beats silently discarding a TryGetParent/TryGetRank
    // result at every internal call site.
    public int GetParent(int id)
        => TryGetParent(id, out var parent) ? parent : ThrowInvalidId(id);

    public void SetParent(int id, int parent)
    {
        if (!TrySetParent(id, parent))
        {
            ThrowInvalidId(id);
        }
    }

    public int GetRank(int id)
        => TryGetRank(id, out var rank) ? rank : ThrowInvalidId(id);

    public void IncrementRank(int id)
    {
        if (!TryIncrementRank(id))
        {
            ThrowInvalidId(id);
        }
    }

    public bool TryGetParent(int id, out int parent)
    {
        if (!IsValidId(id))
        {
            // presumption: allow -- parent is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            parent = default;
            return false;
        }

        parent = _parent[id];
        return true;
    }

    public bool TrySetParent(int id, int parent)
    {
        if (!IsValidId(id))
        {
            return false;
        }

        _parent[id] = parent;
        return true;
    }

    public bool TryGetRank(int id, out int rank)
    {
        if (!IsValidId(id))
        {
            // presumption: allow -- rank is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            rank = default;
            return false;
        }

        rank = _rank[id];
        return true;
    }

    public bool TryIncrementRank(int id)
    {
        if (!IsValidId(id))
        {
            return false;
        }

        _rank[id]++;
        return true;
    }

    private bool IsValidId(int id) => id >= 0 && id < Count;

    private static int ThrowInvalidId(int id) => throw new ArgumentOutOfRangeException(nameof(id), InvalidIdMessage);
}
