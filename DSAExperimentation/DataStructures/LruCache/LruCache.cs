using DSAExperimentation.DataStructures.DoublyLinkedList;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.DataStructures.LruCache;

// Hardwired to exactly one HashMap+DoublyLinkedList composition, no interface, no
// second implementation possible - it is the data structure, with no Representation of
// its own (ARCHITECTURE.md §5 step 7), the same shape Stack.cs composing DynamicArray
// occupies in one file.
//
// No throwing Get, TryGetValue/Set naming: matches HashMap's own policy character-for-
// character, and matters more than it might look here - LruCache and LfuCache are peer
// implementations of an almost-identical public contract, so a reader comparing the two
// should see the same verb for the same semantic (LeetCode's own get/put naming is not
// followed).
//
// O(1) TryGetValue/Set depends on HashMap.TryGetValue/Set/TryRemove being O(1) expected
// and DoublyLinkedList.AddFront/Remove/PopBack being O(1) actual - guaranteed, pointer
// splicing, not hash-dependent (§8).
internal sealed class LruCache<TKey, TValue>
{
    private readonly HashMap<TKey, DoublyLinkedListNode<(TKey Key, TValue Value)>> _nodesByKey = new();
    private readonly DoublyLinkedList<(TKey Key, TValue Value)> _order = new();
    private readonly int _capacity;

    public int Count => _nodesByKey.Count;

    public LruCache(int capacity)
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

        Promote(node);
        value = node.Value.Value;
        return true;
    }

    public void Set(TKey key, TValue value)
    {
        if (_nodesByKey.TryGetValue(key, out var existing))
        {
            existing.Value = (key, value);
            Promote(existing);
            return;
        }

        // _order.Count == _capacity >= 1 here whenever this fires, so PopBack's
        // assume-non-empty contract is a provable invariant, not a runtime
        // uncertainty - the same relationship DisjointSetForest's throwing convenience
        // has to its internal-trusted caller.
        if (_nodesByKey.Count == _capacity)
        {
            var evicted = _order.PopBack();
            _nodesByKey.TryRemove(evicted.Value.Key);
        }

        var node = new DoublyLinkedListNode<(TKey Key, TValue Value)> { Value = (key, value) };
        _order.AddFront(node);
        _nodesByKey.Set(key, node);
    }

    private void Promote(DoublyLinkedListNode<(TKey Key, TValue Value)> node)
    {
        _order.Remove(node);
        _order.AddFront(node);
    }
}
