namespace DSAExperimentation.Collections.HashMap;

// Separate chaining: each bucket holds the index of its chain's first entry (or -1),
// and each entry's Next links to the next entry in the same bucket's chain (or -1 at
// the chain's end). Removal doesn't shift or tombstone anything - the freed slot is
// pushed onto a free list (_freeListHead) and the next Set reuses it before growing
// the entries array, the same technique .NET's own Dictionary<TKey,TValue> uses.
internal sealed class HashMap<TKey, TValue>
{
    private const double MaxLoadFactor = 0.75;
    private const string KeyNotFoundMessage = "The given key was not present in the map.";

    private readonly IEqualityComparer<TKey> _comparer;

    private int[] _buckets = CreateEmptyBuckets(ArrayGrowth.InitialCapacity);
    private HashMapEntry<TKey, TValue>[] _entries = new HashMapEntry<TKey, TValue>[ArrayGrowth.InitialCapacity];
    private int _entryCount;
    private int _freeListHead = -1;
    private int _freeCount;

    public HashMap()
        : this(EqualityComparer<TKey>.Default)
    {
    }

    public HashMap(IEqualityComparer<TKey> comparer)
    {
        _comparer = comparer;
    }

    public int Count => _entryCount - _freeCount;

    public bool ContainsKey(TKey key) => FindEntryIndex(key) >= 0;

    public bool TryGetValue(TKey key, out TValue value)
    {
        var index = FindEntryIndex(key);

        if (index < 0)
        {
            value = default!;
            return false;
        }

        value = _entries[index].Value;
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
        var bucketIndex = hashCode % _buckets.Length;

        if (TryUpdateExisting(hashCode, bucketIndex, key, value))
        {
            return;
        }

        if (Count + 1 > _buckets.Length * MaxLoadFactor)
        {
            Grow();
            bucketIndex = hashCode % _buckets.Length;
        }

        InsertNew(hashCode, bucketIndex, key, value);
    }

    public bool Remove(TKey key)
    {
        var hashCode = ComputeHashCode(key);
        var bucketIndex = hashCode % _buckets.Length;
        var previous = -1;

        for (var i = _buckets[bucketIndex]; i >= 0; i = _entries[i].Next)
        {
            if (_entries[i].HashCode == hashCode && _comparer.Equals(_entries[i].Key, key))
            {
                UnlinkAndFree(i, bucketIndex, previous);
                return true;
            }

            previous = i;
        }

        return false;
    }

    private bool TryUpdateExisting(int hashCode, int bucketIndex, TKey key, TValue value)
    {
        for (var i = _buckets[bucketIndex]; i >= 0; i = _entries[i].Next)
        {
            if (_entries[i].HashCode == hashCode && _comparer.Equals(_entries[i].Key, key))
            {
                _entries[i].Value = value;
                return true;
            }
        }

        return false;
    }

    private void InsertNew(int hashCode, int bucketIndex, TKey key, TValue value)
    {
        var entryIndex = AllocateEntrySlot();

        _entries[entryIndex].HashCode = hashCode;
        _entries[entryIndex].Key = key;
        _entries[entryIndex].Value = value;
        _entries[entryIndex].Next = _buckets[bucketIndex];
        _buckets[bucketIndex] = entryIndex;
    }

    private int AllocateEntrySlot()
    {
        if (_freeCount > 0)
        {
            var reused = _freeListHead;
            _freeListHead = _entries[reused].Next;
            _freeCount--;
            return reused;
        }

        var appended = _entryCount;
        _entryCount++;
        return appended;
    }

    private void UnlinkAndFree(int index, int bucketIndex, int previous)
    {
        if (previous < 0)
        {
            _buckets[bucketIndex] = _entries[index].Next;
        }
        else
        {
            _entries[previous].Next = _entries[index].Next;
        }

        _entries[index].Key = default!;
        _entries[index].Value = default!;
        _entries[index].Next = _freeListHead;
        _freeListHead = index;
        _freeCount++;
    }

    private int ComputeHashCode(TKey key) => _comparer.GetHashCode(key!) & int.MaxValue;

    private int FindEntryIndex(TKey key)
    {
        var hashCode = ComputeHashCode(key);
        var bucketIndex = hashCode % _buckets.Length;

        for (var i = _buckets[bucketIndex]; i >= 0; i = _entries[i].Next)
        {
            if (_entries[i].HashCode == hashCode && _comparer.Equals(_entries[i].Key, key))
            {
                return i;
            }
        }

        return -1;
    }

    private void Grow()
    {
        var oldBuckets = _buckets;
        var oldEntries = _entries;

        var newBuckets = CreateEmptyBuckets(_buckets.Length * ArrayGrowth.GrowthFactor);
        var newEntries = new HashMapEntry<TKey, TValue>[newBuckets.Length];
        var newEntryCount = 0;

        foreach (var bucketHead in oldBuckets)
        {
            for (var i = bucketHead; i >= 0; i = oldEntries[i].Next)
            {
                RehashEntryInto(newBuckets, newEntries, ref newEntryCount, oldEntries[i]);
            }
        }

        _buckets = newBuckets;
        _entries = newEntries;
        _entryCount = newEntryCount;
        _freeListHead = -1;
        _freeCount = 0;
    }

    private static void RehashEntryInto(
        int[] newBuckets, HashMapEntry<TKey, TValue>[] newEntries, ref int newEntryCount, HashMapEntry<TKey, TValue> entry)
    {
        var bucketIndex = entry.HashCode % newBuckets.Length;

        newEntries[newEntryCount].HashCode = entry.HashCode;
        newEntries[newEntryCount].Key = entry.Key;
        newEntries[newEntryCount].Value = entry.Value;
        newEntries[newEntryCount].Next = newBuckets[bucketIndex];
        newBuckets[bucketIndex] = newEntryCount;
        newEntryCount++;
    }

    private static int[] CreateEmptyBuckets(int capacity)
    {
        var buckets = new int[capacity];
        Array.Fill(buckets, -1);
        return buckets;
    }
}
