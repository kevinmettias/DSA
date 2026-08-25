using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Algorithms.HashMap;

internal sealed class HashMap<TKey, TValue>
{
    private const string KeyNotFoundMessage = "The given key was not present in the map.";

    private readonly IEqualityComparer<TKey> _comparer;
    private readonly HashMapStorage<TKey, TValue> _storage = new();

    public HashMap()
        : this(EqualityComparer<TKey>.Default)
    {
    }

    public HashMap(IEqualityComparer<TKey> comparer)
    {
        _comparer = comparer;
    }

    public int Count => _storage.Count;

    public bool ContainsKey(TKey key) => FindEntryIndex(key) >= 0;

    public bool TryGetValue(TKey key, out TValue value)
    {
        var index = FindEntryIndex(key);

        if (index < 0)
        {
            value = default!;
            return false;
        }

        value = _storage.EntryValue(index);
        return true;
    }

    // A convenience throwing form beside TryGetValue's recoverable one, the same
    // pairing Heap.Peek/Stack.Peek/Deque.PeekFront/Queue.Peek already use for their
    // own "empty" precondition.
    public TValue Get(TKey key)
        => TryGetValue(key, out var value)
            ? value
            : throw new KeyNotFoundException(KeyNotFoundMessage);

    public void Set(TKey key, TValue value)
    {
        var hashCode = ComputeHashCode(key);
        var bucketIndex = _storage.BucketIndexFor(hashCode);

        if (TryUpdateExisting(hashCode, bucketIndex, key, value))
        {
            return;
        }

        _storage.Insert(hashCode, key, value);
    }

    public bool Remove(TKey key)
    {
        var hashCode = ComputeHashCode(key);
        var bucketIndex = _storage.BucketIndexFor(hashCode);
        var previous = -1;

        for (var i = _storage.BucketHead(bucketIndex); i >= 0; i = _storage.EntryNext(i))
        {
            if (_storage.EntryHashCode(i) == hashCode && _comparer.Equals(_storage.EntryKey(i), key))
            {
                _storage.Unlink(bucketIndex, previous, i);
                return true;
            }

            previous = i;
        }

        return false;
    }

    private bool TryUpdateExisting(int hashCode, int bucketIndex, TKey key, TValue value)
    {
        for (var i = _storage.BucketHead(bucketIndex); i >= 0; i = _storage.EntryNext(i))
        {
            if (_storage.EntryHashCode(i) == hashCode && _comparer.Equals(_storage.EntryKey(i), key))
            {
                _storage.SetEntryValue(i, value);
                return true;
            }
        }

        return false;
    }

    private int ComputeHashCode(TKey key) => _comparer.GetHashCode(key!) & int.MaxValue;

    private int FindEntryIndex(TKey key)
    {
        var hashCode = ComputeHashCode(key);
        var bucketIndex = _storage.BucketIndexFor(hashCode);

        for (var i = _storage.BucketHead(bucketIndex); i >= 0; i = _storage.EntryNext(i))
        {
            if (_storage.EntryHashCode(i) == hashCode && _comparer.Equals(_storage.EntryKey(i), key))
            {
                return i;
            }
        }

        return -1;
    }
}
