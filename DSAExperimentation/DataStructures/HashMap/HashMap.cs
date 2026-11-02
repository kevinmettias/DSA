using System.Runtime.CompilerServices;

namespace DSAExperimentation.DataStructures.HashMap;

// No throwing Get convenience: whether a key is present is state an external caller
// can't always know in advance, so TryGetValue is the only public surface for
// reading a value, forcing callers onto the return-value form instead of a
// try/catch.
/// <summary>
/// A hash map whose value for a key is read only through <see cref="TryGetValue"/>, so a missing key never throws.
/// </summary>
public sealed class HashMap<TKey, TValue>
{
    // Null when TKey is a value type and the comparer is EqualityComparer<TKey>.Default, the way
    // Dictionary<TKey,TValue> stores it. IsSameKey and KeyHashCode then call Default itself, which
    // the JIT devirtualizes and inlines for a value-type key; a call through the stored interface
    // is one it cannot inline (ARCHITECTURE 14.2). A reference-type key keeps its comparer, since
    // its code is shared across every reference type and Default would be no more direct.
    private readonly IEqualityComparer<TKey>? _comparer;
    private readonly HashMapStorage<TKey, TValue> _storage = new();

    /// <summary>How many entries the map holds.</summary>
    public int Count => _storage.Count;

    // Eager snapshots (via HashMapStorage.SnapshotEntries), not a live view - mutating
    // the map after reading Keys/Values, or mid-foreach, has no effect on the already
    // -copied result. Only Keys/Values are exposed, not a KeyValuePair/GetEnumerator
    // pairing - that's the named gap this addition closes; a Pairs-shaped property
    // would be a one-line projection over the same snapshot if a caller ever needs it.
    /// <summary>A copy of the map's keys taken when read, unaffected by later changes to the map.</summary>
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

    /// <summary>A copy of the map's values taken when read, unaffected by later changes to the map.</summary>
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

    public HashMap(IEqualityComparer<TKey> comparer) => _comparer = StoredComparer(comparer);

    private static IEqualityComparer<TKey>? StoredComparer(IEqualityComparer<TKey> comparer) =>
        typeof(TKey).IsValueType && ReferenceEquals(comparer, EqualityComparer<TKey>.Default) ? null : comparer;

    /// <summary>Reports whether the map holds a key.</summary>
    /// <param name="key">The key to look for.</param>
    /// <returns>True when the map holds <paramref name="key"/>; otherwise false.</returns>
    public bool HasKey(TKey key) => FindEntryIndex(key) >= 0;

    /// <summary>Looks up the value stored under a key.</summary>
    /// <param name="key">The key to look up.</param>
    /// <param name="value">
    /// The value stored under <paramref name="key"/> when this returns true; meaningful only then.
    /// </param>
    /// <returns>True when the map holds <paramref name="key"/>; otherwise false.</returns>
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

    /// <summary>Stores a value under a key, replacing any value the key already had.</summary>
    /// <param name="key">The key to store under.</param>
    /// <param name="value">The value to store.</param>
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

            if (entry.HashCode == hashCode && IsSameKey(entry.Key, key))
            {
                _storage.SetEntryValue(i, value);
                return true;
            }

            i = entry.Next;
        }

        return false;
    }

    /// <summary>Removes a key and its value, if the map holds the key.</summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>True when the key was present and is now removed; false when the map did not hold it.</returns>
    public bool TryRemove(TKey key)
    {
        var hashCode = ComputeHashCode(key);
        var bucketIndex = _storage.BucketIndexFor(hashCode);
        var previous = -1;
        var i = _storage.BucketHead(bucketIndex);

        while (i >= 0)
        {
            var entry = _storage.Entry(i);

            if (entry.HashCode == hashCode && IsSameKey(entry.Key, key))
            {
                _storage.Unlink(bucketIndex, previous, i);
                return true;
            }

            previous = i;
            i = entry.Next;
        }

        return false;
    }

    private int ComputeHashCode(TKey key) => KeyHashCode(key) & int.MaxValue;

    // presumption: allow -- key! exists only because TKey is unconstrained, so the
    // compiler cannot see a reference-type key is non-null; a literal null key's
    // actual behavior is the comparer's own contract (EqualityComparer<TKey>.Default
    // treats null as a valid key, hashing it to 0), not something this map decides.
    // _comparer! holds because _comparer is null only for a value-type key, which the
    // first arm takes; see IsSameKey for why the test reads TKey first.
    private int KeyHashCode(TKey key) =>
        typeof(TKey).IsValueType && _comparer is null
            ? EqualityComparer<TKey>.Default.GetHashCode(key!)
            : _comparer!.GetHashCode(key!);

    private int FindEntryIndex(TKey key)
    {
        var hashCode = ComputeHashCode(key);
        var bucketIndex = _storage.BucketIndexFor(hashCode);
        var i = _storage.BucketHead(bucketIndex);

        while (i >= 0)
        {
            var entry = _storage.Entry(i);

            if (entry.HashCode == hashCode && IsSameKey(entry.Key, key))
            {
                return i;
            }

            i = entry.Next;
        }

        return -1;
    }

    // Always inlined. Without the attribute the JIT judged this an "unprofitable inline" in
    // FindEntryIndex, read with the inlining verb (ARCHITECTURE 19.8) on LC 874's SetObstacles arm,
    // so every probe of a chain still made a call, though Default.Equals inlined inside it. The
    // test reads TKey first, as Dictionary's does: the JIT folds it, so a reference-type key's code
    // never holds the Default arm, and is the interface call it always was. With the arm present
    // but dead, GroupAnagrams' string-keyed arm allocated 31 B more per call.
    // presumption: allow -- _comparer is null only for a value-type key, which the first arm takes.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSameKey(TKey stored, TKey key) =>
        typeof(TKey).IsValueType && _comparer is null
            ? EqualityComparer<TKey>.Default.Equals(stored, key)
            : _comparer!.Equals(stored, key);
}
