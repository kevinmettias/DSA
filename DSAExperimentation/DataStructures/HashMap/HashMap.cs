
namespace DSAExperimentation.DataStructures.HashMap;

// No throwing Get convenience: whether a key is present is state an external caller
// can't always know in advance, so TryGetValue is the only public surface for
// reading a value, forcing callers onto the return-value form instead of a
// try/catch.
internal sealed class HashMap<TKey, TValue>
{
    private readonly IEqualityComparer<TKey> _comparer;
    private readonly HashMapStorage<TKey, TValue> _storage = new();

    public int Count => _storage.Count;

    // Eager snapshots (via HashMapStorage.SnapshotEntries), not a live view - mutating
    // the map after reading Keys/Values, or mid-foreach, has no effect on the already
    // -copied result. Only Keys/Values are exposed, not a KeyValuePair/GetEnumerator
    // pairing - that's the named gap this addition closes; a Pairs-shaped property
    // would be a one-line projection over the same snapshot if a caller ever needs it.
    public IEnumerable<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>(_storage.Count);

            foreach (var entry in _storage.SnapshotEntries())
            {
                keys.Add(entry.Key);
            }

            return keys;
        }
    }

    public IEnumerable<TValue> Values
    {
        get
        {
            var values = new List<TValue>(_storage.Count);

            foreach (var entry in _storage.SnapshotEntries())
            {
                values.Add(entry.Value);
            }

            return values;
        }
    }

    public HashMap()
        : this(EqualityComparer<TKey>.Default)
    {
    }

    public HashMap(IEqualityComparer<TKey> comparer) => _comparer = comparer;

    public bool HasKey(TKey key) => FindEntryIndex(key) >= 0;

    public bool TryGetValue(TKey key, out TValue value)
    {
        var index = FindEntryIndex(key);

        if (index < 0)
        {
            // presumption: allow -- value is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            value = default!;
            return false;
        }

        value = _storage.Entry(index).Value;
        return true;
    }

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

    private bool TryUpdateExisting(int hashCode, int bucketIndex, TKey key, TValue value)
    {
        var i = _storage.BucketHead(bucketIndex);

        while (i >= 0)
        {
            var entry = _storage.Entry(i);

            if (entry.HashCode == hashCode && _comparer.Equals(entry.Key, key))
            {
                _storage.SetEntryValue(i, value);
                return true;
            }

            i = entry.Next;
        }

        return false;
    }

    public bool TryRemove(TKey key)
    {
        var hashCode = ComputeHashCode(key);
        var bucketIndex = _storage.BucketIndexFor(hashCode);
        var previous = -1;
        var i = _storage.BucketHead(bucketIndex);

        while (i >= 0)
        {
            var entry = _storage.Entry(i);

            if (entry.HashCode == hashCode && _comparer.Equals(entry.Key, key))
            {
                _storage.Unlink(bucketIndex, previous, i);
                return true;
            }

            previous = i;
            i = entry.Next;
        }

        return false;
    }

    // presumption: allow -- key! exists only because TKey is unconstrained, so the
    // compiler cannot see a reference-type key is non-null; a literal null key's
    // actual behavior is the comparer's own contract (EqualityComparer<TKey>.Default
    // treats null as a valid key, hashing it to 0), not something this map decides.
    private int ComputeHashCode(TKey key) => _comparer.GetHashCode(key!) & int.MaxValue;

    private int FindEntryIndex(TKey key)
    {
        var hashCode = ComputeHashCode(key);
        var bucketIndex = _storage.BucketIndexFor(hashCode);
        var i = _storage.BucketHead(bucketIndex);

        while (i >= 0)
        {
            var entry = _storage.Entry(i);

            if (entry.HashCode == hashCode && _comparer.Equals(entry.Key, key))
            {
                return i;
            }

            i = entry.Next;
        }

        return -1;
    }
}
