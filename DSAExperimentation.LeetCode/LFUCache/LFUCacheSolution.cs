using DSAExperimentation.DataStructures.Cache;
using DSAExperimentation.DataStructures.Cache.LfuCache;

namespace DSAExperimentation.LeetCode.LFUCache;

// LeetCode 460. LFU Cache: a fixed-capacity get/put cache that evicts the least
// frequently used key on overflow, ties broken by least recently used, both
// operations O(1).
//
// This is a design problem - LeetCode's own shape is a stateful object with a
// constructor and two operations, not a single return value - so the strategy
// choice is which cache implementation backs it, and each factory hands back
// this repo's ICache<TKey, TValue> shape (TryGetValue/Set, not get/put -
// LfuCache's own doc comment already rejects LeetCode's verbs in favor of
// matching HashMap's naming, the same precedent LRUCacheSolution follows for
// its peer cache). The two arms answer the same LeetCode operation sequence,
// replayed by the harness.
internal static class LFUCacheSolution
{
    // The composed answer: this repo's own HashMap + DoublyLinkedList frequency
    // buckets, already under direct test at DataStructures/Cache/LfuCache.
    public static ICache<int, int> CreateByLfuCachePrimitive(int capacity) =>
        new LfuCache<int, int>(capacity);

    // The textbook baseline this composition has to justify itself against:
    // a BCL Dictionary keyed by the cache key, with frequency and a
    // monotonic tick (for the least-recently-used tie-break) carried right
    // in the entry, and eviction found by a linear scan - O(n) per eviction
    // instead of the primitive's O(1).
    public static ICache<int, int> CreateByDictionaryLinearScan(int capacity) =>
        new DictionaryLinearScanCache(capacity);

    private sealed class DictionaryLinearScanCache(int capacity) : ICache<int, int>
    {
        private readonly Dictionary<int, (int Value, int Frequency, long Tick)> _entries = [];
        private long _clock;

        public int Count => _entries.Count;

        public bool TryGetValue(int key, out int value)
        {
            if (!_entries.TryGetValue(key, out var entry))
            {
                value = default;
                return false;
            }

            _entries[key] = (entry.Value, entry.Frequency + 1, _clock++);
            value = entry.Value;
            return true;
        }

        public void Set(int key, int value)
        {
            if (_entries.TryGetValue(key, out var existing))
            {
                _entries[key] = (value, existing.Frequency + 1, _clock++);
                return;
            }

            if (_entries.Count == capacity)
            {
                EvictLeastFrequentlyUsed();
            }

            _entries[key] = (value, 1, _clock++);
        }

        private void EvictLeastFrequentlyUsed()
        {
            var evictKey = 0;
            var minFrequency = int.MaxValue;
            var oldestTick = long.MaxValue;

            foreach (var (candidateKey, entry) in _entries)
            {
                if (entry.Frequency < minFrequency ||
                    (entry.Frequency == minFrequency && entry.Tick < oldestTick))
                {
                    evictKey = candidateKey;
                    minFrequency = entry.Frequency;
                    oldestTick = entry.Tick;
                }
            }

            _entries.Remove(evictKey);
        }
    }
}
