using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.DataStructures.DisjointSet;

// §10.4 anticipates this exact gap and prescribes composing DisjointSet plus an
// id-assignment map rather than reimplementing Find/Union - "composition over redesign,"
// the same relationship Stack/Queue have with DynamicArray/Deque (§4.1). §10.4's own
// text names the wrapper DisjointSet<T>, but this repo's one-file-per-type convention
// (every Collections/** file is named after its own type - Heap.cs -> Heap<Element,
// TOrder>, HashMap.cs -> HashMap<TKey,TValue>) can't give two files the same name, so
// this type is KeyedDisjointSet<TKey> instead, living beside DisjointSet.cs/
// DisjointSetForest.cs since it introduces no new Representation or Topology (§5 step 7).
//
// Composes DisjointSet (the Operations layer - never DisjointSetForest directly, so
// Find/Union's path-compression/union-by-rank is never re-derived) plus this repo's own
// HashMap<TKey,int> and DynamicArray<TKey>, not BCL Dictionary/List: §5 step 5 only
// forbids imposing your own contract on another domain's Representation, not consuming
// an already-public concrete type the way Stack composes DynamicArray or Set composes
// HashMap<T,bool> (§4.1); and §7 documents preferring an already-existing self-hosted
// structure over its BCL equivalent once one exists (ShortestPath switching from BCL
// PriorityQueue to Collections/Heap). No using alias is needed for the bare DisjointSet
// reference below, unlike DisjointSetTests.cs (§10.3) - that alias is only needed when a
// *different* namespace imports the type via `using`; this file is declared directly
// inside the same DataStructures.DisjointSet namespace DisjointSet itself lives in, so
// ordinary same-namespace lookup finds it with no ambiguity.
//
// TryFind/TryUnion/IsConnected's O(a(n)) amortized claim additionally depends on
// HashMap.TryGetValue/HasKey being O(1) expected and DynamicArray.Get being O(1) (§8's
// actionable rule - the same cross-reference Heap.cs/HeapArray.cs carry for their own
// O(log n) claim).
//
// Fixed key universe at construction, no growth after - the same "known upfront" shape
// DisjointSetForest itself already commits to. This is a locally-scoped choice for the
// consumers this type targets (Accounts Merge, Evaluate Division, Redundant Connection
// with string labels all obtain their full key universe from one upfront pass over the
// input before any Union call), not a claim that §10.4 mandates it either way.
//
// Try-only public surface, no throwing Find/Union - the opposite choice from the
// DisjointSet/DisjointSetForest pair it wraps, and deliberately so. DisjointSetForest's
// throwing convenience exists because an invalid id is "DisjointSet.cs's own
// internal-trusted caller's bug" (DisjointSetForest.cs). This type's whole purpose is
// bridging to keys whose membership is genuinely runtime-uncertain to an external caller
// (Evaluate Division querying a variable never seen in any equation) - the same shape
// HashMap.cs's own doc comment names for why it has no throwing Get. The construction-
// time key universe above and a query-time key here are two different sets: a key can be
// legitimately unknown to a query even though the type itself never grows.
internal sealed class KeyedDisjointSet<TKey>
{
    private readonly HashMap<TKey, int> _index;
    private readonly DynamicArray<TKey> _keysById;
    private readonly DisjointSet _disjointSet;

    public int Count => _keysById.Count;

    public KeyedDisjointSet(IEnumerable<TKey> keys)
        : this(keys, EqualityComparer<TKey>.Default)
    {
    }

    public KeyedDisjointSet(IEnumerable<TKey> keys, IEqualityComparer<TKey> comparer)
    {
        _index = new HashMap<TKey, int>(comparer);
        _keysById = new DynamicArray<TKey>();

        foreach (var key in keys)
        {
            if (_index.HasKey(key))
            {
                continue;
            }

            _index.Set(key, _keysById.Count);
            _keysById.Add(key);
        }

        _disjointSet = new DisjointSet(_keysById.Count);
    }

    public bool HasKey(TKey key) => _index.HasKey(key);

    public bool TryFind(TKey key, out TKey representative)
    {
        if (!_index.TryGetValue(key, out var id))
        {
            // presumption: allow -- representative is only meaningful when this returns
            // true, the standard TryGetValue/TryParse out-parameter contract this mirrors.
            representative = default!;
            return false;
        }

        representative = _keysById.Get(_disjointSet.Find(id));
        return true;
    }

    public bool TryUnion(TKey first, TKey second)
    {
        if (!_index.TryGetValue(first, out var firstId) || !_index.TryGetValue(second, out var secondId))
        {
            return false;
        }

        _disjointSet.Union(firstId, secondId);
        return true;
    }

    public bool IsConnected(TKey first, TKey second)
    {
        if (!_index.TryGetValue(first, out var firstId) || !_index.TryGetValue(second, out var secondId))
        {
            return false;
        }

        return _disjointSet.IsConnected(firstId, secondId);
    }
}
