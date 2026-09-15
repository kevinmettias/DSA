using System.Diagnostics.CodeAnalysis;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;
using DisjointSetOperations = DSAExperimentation.DataStructures.DisjointSet.DisjointSet;

namespace DSAExperimentation.DataStructures.KeyedDisjointSet;

// Own folder, not nested inside DataStructures/DisjointSet/: §4.1's table shows every
// composing wrapper in this repo (Stack/DynamicArray, Queue/Deque, Set/HashMap) gets its
// own top-level folder distinct from what it composes - a composing wrapper is itself "a
// Structure," unlike a type's own dedicated, no-other-purpose Representation (HeapArray,
// which has no identity outside Heap and so stays inside Heap/'s folder). §5 step 7's
// "co-locates under DataStructures/<Structure>/" means the wrapper's own name, not the
// name of what it wraps.
//
// §10.4 anticipates this exact gap and prescribes composing DisjointSet plus an
// id-assignment map rather than reimplementing Find/Union - "composition over redesign,"
// the same relationship Stack/Queue already have with DynamicArray/Deque (§4.1). §10.4's
// own text names the wrapper DisjointSet<T>, but this repo's one-file-per-type convention
// (every Collections/** file is named after its own type - Heap.cs -> Heap<Element,
// TOrder>, HashMap.cs -> HashMap<TKey,TValue>) can't give two files the same name, so
// this type is KeyedDisjointSet<TKey> instead.
//
// Composes DisjointSet (the Operations layer - never DisjointSetForest directly, so
// Find/Union's path-compression/union-by-rank is never re-derived), aliased to
// DisjointSetOperations for the same reason DisjointSetTests.cs (§10.3) needs it: this
// file's own namespace differs from DisjointSet.cs's, so a `using` is required to reach
// it, and that using collides with the sibling namespace segment sharing the same name -
// a same-named alias wouldn't help, only a differently-named one does.
//
// Also composes this repo's own HashMap<TKey,int> and DynamicArray<TKey>, not BCL
// Dictionary/List: §5 step 5 only
// forbids imposing your own contract on another domain's Representation, not consuming
// an already-public concrete type the way Stack composes DynamicArray or Set composes
// HashMap<T,bool> (§4.1); and §7 documents preferring an already-existing self-hosted
// structure over its BCL equivalent once one exists (ShortestPath switching from BCL
// PriorityQueue to Collections/Heap).
//
// TryFind/TryUnion/IsConnected's O(a(n)) amortized claim additionally depends on
// HashMap.TryGetValue/HasKey being O(1) expected and DynamicArray.Get being O(1) (§8's
// actionable rule - the same cross-reference Heap.cs/HeapArray.cs carry for their own
// O(log n) claim).
//
// Fixed key universe at construction, no growth after - the same "known upfront" shape
// DisjointSetForest itself already commits to. This is a locally-scoped choice for the
// consumers this type targets (Accounts Merge, Evaluate Division, Redundant Connection
// with string labels, and a Kruskal's-MST caller's vertex set all obtain their full key
// universe from one upfront pass over the input before any Union call), not a claim that
// §10.4 mandates it either way.
//
// Try-only public surface, no throwing Find/Union - the opposite choice from the
// DisjointSet/DisjointSetForest pair it wraps, and deliberately so. DisjointSetForest's
// throwing convenience exists because an invalid id is "DisjointSet.cs's own
// internal-trusted caller's bug" (DisjointSetForest.cs). This type's whole purpose is
// bridging to keys whose membership is genuinely runtime-uncertain to an external caller
// (Evaluate Division querying a variable never seen in any equation; a Kruskal's-MST
// caller discovering an edge whose neighbor falls outside the given vertex set) - the
// same shape HashMap.cs's own doc comment names for why it has no throwing Get. The
// construction-time key universe above and a query-time key here are two different sets:
// a key can be legitimately unknown to a query even though the type itself never grows.
internal sealed class KeyedDisjointSet<TKey>
{
    private readonly HashMap<TKey, int> _index;
    private readonly DynamicArray<TKey> _keysById;
    private readonly DisjointSetOperations _disjointSet;

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

        _disjointSet = new DisjointSetOperations(_keysById.Count);
    }

    public bool HasKey(TKey key) => _index.HasKey(key);

    // [MaybeNullWhen(false)] is what lets the absent branch assign `default` rather than
    // `default!`: it tells the compiler the out-parameter is only meaningfully read when
    // this returns true, so the contract is declared in the signature instead of being
    // asserted at the assignment.
    public bool TryFind(TKey key, [MaybeNullWhen(false)] out TKey representative)
    {
        if (!_index.TryGetValue(key, out var id))
        {
            representative = default;
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
