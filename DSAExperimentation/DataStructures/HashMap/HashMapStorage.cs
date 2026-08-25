namespace DSAExperimentation.DataStructures.HashMap;

// Raw bucket/entry mechanics only - no comparer, no equality logic. HashMap owns the
// comparer-dependent chain walks (TryUpdateExisting/FindEntryIndex/Remove); this type
// owns everything that doesn't need to compare keys: bucket indexing, the free list,
// and growing. Separate chaining: each bucket holds the index of its chain's first
// entry (or -1), and each entry's Next links to the next entry in the same bucket's
// chain (or -1 at the chain's end). Removal doesn't shift or tombstone anything - the
// freed slot is pushed onto a free list (_freeListHead) and the next Insert reuses it
// before growing the entries array, the same technique .NET's own
// Dictionary<TKey,TValue> uses.
internal sealed class HashMapStorage<TKey, TValue>
{
    private const double MaxLoadFactor = 0.75;

    private int[] _buckets = CreateEmptyBuckets(ArrayGrowth.InitialCapacity);
    private HashMapEntry<TKey, TValue>[] _entries = new HashMapEntry<TKey, TValue>[ArrayGrowth.InitialCapacity];
    private int _entryCount;
    private int _freeListHead = -1;
    private int _freeCount;

    public int Count => _entryCount - _freeCount;

    public int BucketIndexFor(int hashCode) => hashCode % _buckets.Length;

    public int BucketHead(int bucketIndex) => _buckets[bucketIndex];

    public int EntryNext(int index) => _entries[index].Next;

    public int EntryHashCode(int index) => _entries[index].HashCode;

    public TKey EntryKey(int index) => _entries[index].Key;

    public TValue EntryValue(int index) => _entries[index].Value;

    public void SetEntryValue(int index, TValue value) => _entries[index].Value = value;

    // Absorbs the whole grow-if-needed -> allocate -> link pipeline: "should this
    // insert grow first" isn't a comparer-dependent decision, so it belongs here in
    // full. Recomputes the bucket index itself, after any grow, rather than accepting
    // one from the caller - the only way to guarantee it's never stale.
    public void Insert(int hashCode, TKey key, TValue value)
    {
        if (Count + 1 > _buckets.Length * MaxLoadFactor)
        {
            Grow();
        }

        var bucketIndex = BucketIndexFor(hashCode);
        var entryIndex = AllocateEntrySlot();

        _entries[entryIndex].HashCode = hashCode;
        _entries[entryIndex].Key = key;
        _entries[entryIndex].Value = value;
        _entries[entryIndex].Next = _buckets[bucketIndex];
        _buckets[bucketIndex] = entryIndex;
    }

    public void Unlink(int bucketIndex, int previous, int index)
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
