namespace DSAExperimentation.DataStructures.HashMap;

// Mutable by design, not a data-bag class: instances live inside HashMap's entries
// array and are updated in place through the array indexer (T[] indexing yields a
// real variable for a value type, unlike List<T>'s indexer). Next threads two
// different chains depending on context - the bucket's chain when the slot is live,
// the free list when it isn't - never both at once.
internal struct HashMapEntry<TKey, TValue>
{
    public int HashCode;
    public int Next;
    public TKey Key;
    public TValue Value;
}
