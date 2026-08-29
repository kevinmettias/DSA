namespace DSAExperimentation.DataStructures.Cache;

// LruCache/LfuCache are peer implementations of this identical public contract (their own doc
// comments already say so - see LruCache.cs's comment on why the verb choice matches). Extracted
// as an interface, not a shared base class, per ARCHITECTURE.md §6's composition-over-inheritance
// precedent (IHeapOrder, ICombineOperation, IGroupOperation are all interfaces, never base
// classes) - the two caches share no state or method bodies to inherit, only this public shape,
// and their eviction policies produce genuinely different observable output for the same call
// sequence (the §10.1 discriminator for a real, caller-visible axis), the same way MinHeapOrder
// and MaxHeapOrder do - unlike, say, DisjointSet's linking policy, which never changes what a
// caller observes.
internal interface ICache<TKey, TValue>
{
    int Count { get; }

    bool TryGetValue(TKey key, out TValue value);

    void Set(TKey key, TValue value);
}
