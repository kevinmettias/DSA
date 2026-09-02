using DSAExperimentation.DataStructures.Cache;
using DSAExperimentation.DataStructures.Cache.LruCache;

namespace DSAExperimentation.LeetCode.LRUCache;

// LeetCode 146. LRU Cache: a fixed-capacity get/put cache that evicts the least
// recently used key on overflow, both operations O(1).
//
// This is a design problem - LeetCode's own shape is a stateful object with a
// constructor and two operations, not a single return value - so the strategy
// choice is which cache implementation backs it, and each factory hands back
// this repo's ICache<TKey, TValue> shape (TryGetValue/Set, not get/put -
// LruCache's own doc comment already rejects LeetCode's verbs in favor of
// matching HashMap's naming, and that precedent carries over here). The two
// arms answer the same LeetCode operation sequence, replayed by the harness.
internal static class LRUCacheSolution
{
    // The composed answer: this repo's own HashMap + DoublyLinkedList cache,
    // already under direct test at DataStructures/Cache/LruCache.
    public static ICache<int, int> CreateByLruCachePrimitive(int capacity) =>
        new LruCache<int, int>(capacity);

    // The textbook baseline this composition has to justify itself against:
    // BCL Dictionary for O(1) lookup, BCL LinkedList for O(1) recency
    // reordering, nothing from this repo.
    public static ICache<int, int> CreateByDictionaryLinkedList(int capacity) =>
        new DictionaryLinkedListCache(capacity);

    private sealed class DictionaryLinkedListCache(int capacity) : ICache<int, int>
    {
        private readonly Dictionary<int, LinkedListNode<(int Key, int Value)>> _nodesByKey = [];
        private readonly LinkedList<(int Key, int Value)> _order = new();

        public int Count => _nodesByKey.Count;

        public bool TryGetValue(int key, out int value)
        {
            if (!_nodesByKey.TryGetValue(key, out var node))
            {
                value = default;
                return false;
            }

            Promote(node);
            value = node.Value.Value;
            return true;
        }

        public void Set(int key, int value)
        {
            if (_nodesByKey.TryGetValue(key, out var existing))
            {
                existing.Value = (key, value);
                Promote(existing);
                return;
            }

            if (_nodesByKey.Count == capacity)
            {
                var evicted = _order.Last!;
                _order.RemoveLast();
                _nodesByKey.Remove(evicted.Value.Key);
            }

            var node = _order.AddFirst((key, value));
            _nodesByKey[key] = node;
        }

        private void Promote(LinkedListNode<(int Key, int Value)> node)
        {
            _order.Remove(node);
            _order.AddFirst(node);
        }
    }
}
