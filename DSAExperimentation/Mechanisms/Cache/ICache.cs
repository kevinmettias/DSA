namespace DSAExperimentation.Mechanisms.Cache;

// LruCache/LfuCache are peer implementations of this identical public contract (their own doc
// comments already say so - see LruCache.cs's comment on why the verb choice matches). Extracted
// as an interface, not a shared base class, per ARCHITECTURE.md §6's composition-over-inheritance
// precedent (IHeapOrder, IMonoid, IAbelianGroup are all interfaces, never base
// classes) - the two caches share no state or method bodies to inherit, only this public shape,
// and their eviction policies produce genuinely different observable output for the same call
// sequence (the §10.1 discriminator for a real, caller-visible axis), the same way MinHeapOrder
// and MaxHeapOrder do - unlike, say, DisjointSet's linking policy, which never changes what a
// caller observes.
/// <summary>
/// A key-value cache of fixed capacity whose eviction policy chooses the entry that makes room for a new key.
/// </summary>
public interface ICache<TKey, TValue>
{
    /// <summary>The number of entries the cache holds, never more than its capacity.</summary>
    int Count { get; }

    /// <summary>Looks up a key, counting a hit as a use of that entry for the eviction policy.</summary>
    /// <param name="key">The key to look up.</param>
    /// <param name="value">The cached value when the key is present; otherwise the default value.</param>
    /// <returns>True when the key is cached; false, changing nothing, when it is not.</returns>
    bool TryGetValue(TKey key, out TValue value);

    /// <summary>
    /// Stores a value under a key as a use of that entry, first evicting one entry when the key is new and
    /// the cache is full.
    /// </summary>
    /// <param name="key">The key to store under.</param>
    /// <param name="value">The value to store, replacing any value already cached under the key.</param>
    void Set(TKey key, TValue value);
}
