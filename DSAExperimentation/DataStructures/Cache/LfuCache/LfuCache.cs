using DSAExperimentation.DataStructures.DoublyLinkedList;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.DataStructures.Cache.LfuCache;

// Pure Operations, composing two HashMap shapes and DoublyLinkedList, no Representation
// of its own - the same "compose existing Representations, introduce none" shape
// KeyedDisjointSet already uses (ARCHITECTURE.md §5 step 7). _minFrequency is
// Operations-internal bookkeeping, not a Topology witness: there is exactly one correct
// way to track it, no MinHeapOrder-vs-MaxHeapOrder-style variant choice a caller could
// supply, the same closed-no-variant reasoning that keeps DisjointSet's linking policy
// and CheckedFold's in-progress set out of the Topology axis too.
//
// Node payload bundles key+value+frequency together (the same idiom ShortestPath's
// Heap<(TNode Node, TWeight Priority), ...> already establishes) rather than three
// parallel HashMaps that must be kept in lockstep - a key's current value and frequency
// are both reachable in O(1) via the one node _nodesByKey maps to.
//
// TryGetValue/Set naming matches LruCache and HashMap - see LruCache.cs's own comment
// for why matching the verb across these two peer caches is the right call here.
internal sealed class LfuCache<TKey, TValue> : ICache<TKey, TValue>
{
    private readonly HashMap<TKey, DoublyLinkedListNode<(TKey Key, TValue Value, int Frequency)>> _nodesByKey = new();
    private readonly HashMap<int, DoublyLinkedList<(TKey Key, TValue Value, int Frequency)>> _bucketsByFrequency = new();
    private readonly int _capacity;
    private int _minFrequency;

    public int Count => _nodesByKey.Count;

    public LfuCache(int capacity)
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, CacheConstants.InvalidCapacityMessage);
        }

        _capacity = capacity;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (!_nodesByKey.TryGetValue(key, out var node))
        {
            // presumption: allow -- value is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            value = default!;
            return false;
        }

        Touch(node);
        value = node.Value.Value;
        return true;
    }

    public void Set(TKey key, TValue value)
    {
        if (_nodesByKey.TryGetValue(key, out var existing))
        {
            existing.Value = (existing.Value.Key, value, existing.Value.Frequency);
            Touch(existing);
            return;
        }

        if (_nodesByKey.Count == _capacity)
        {
            EvictLeastFrequentlyUsed();
        }

        var node = new DoublyLinkedListNode<(TKey Key, TValue Value, int Frequency)> { Value = (key, value, 1) };
        GetOrCreateBucket(1).AddFront(node);
        _nodesByKey.Set(key, node);
        _minFrequency = 1;
    }

    // _bucketsByFrequency[_minFrequency] is guaranteed present and non-empty whenever
    // this is called - Touch/Set never leave an empty bucket behind (see the cleanup
    // below), so _minFrequency always names a real, populated bucket while the cache is
    // at capacity. Trusting that invariant here (no TryGetValue check) is the same
    // posture DoublyLinkedList.PopBack/Remove already take toward their own callers.
    private void EvictLeastFrequentlyUsed()
    {
        _bucketsByFrequency.TryGetValue(_minFrequency, out var minBucket);
        var evicted = minBucket.PopBack();

        if (minBucket.Count == 0)
        {
            _bucketsByFrequency.TryRemove(_minFrequency);
        }

        _nodesByKey.TryRemove(evicted.Value.Key);
    }

    // Frequency is monotonically non-decreasing from 1 for every key, so a bucket's
    // emptiness is only ever discovered here or in Set's eviction path - both clean it
    // up immediately, keeping _bucketsByFrequency free of dead entries (an unbounded
    // space leak otherwise, despite every operation staying O(1) in time - the
    // space-axis counterpart to §8's performance-independence warning).
    private void Touch(DoublyLinkedListNode<(TKey Key, TValue Value, int Frequency)> node)
    {
        var oldFrequency = node.Value.Frequency;

        // Same trusted invariant as Set's eviction path: a node's own recorded
        // frequency always names a live bucket, since GetOrCreateBucket creates one in
        // the same call that ever assigns that frequency to a node.
        _bucketsByFrequency.TryGetValue(oldFrequency, out var oldBucket);
        oldBucket.Remove(node);

        if (oldBucket.Count == 0)
        {
            _bucketsByFrequency.TryRemove(oldFrequency);

            if (oldFrequency == _minFrequency)
            {
                _minFrequency++;
            }
        }

        var newFrequency = oldFrequency + 1;
        node.Value = (node.Value.Key, node.Value.Value, newFrequency);
        GetOrCreateBucket(newFrequency).AddFront(node);
    }

    private DoublyLinkedList<(TKey Key, TValue Value, int Frequency)> GetOrCreateBucket(int frequency)
    {
        if (_bucketsByFrequency.TryGetValue(frequency, out var bucket))
        {
            return bucket;
        }

        bucket = new DoublyLinkedList<(TKey Key, TValue Value, int Frequency)>();
        _bucketsByFrequency.Set(frequency, bucket);
        return bucket;
    }
}
