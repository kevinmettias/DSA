namespace DSAExperimentation.DataStructures.HashMap;

// Immutable by design: instances live inside HashMap's entries array, and
// HashMapStorage replaces a slot's whole value (`_entries[i] = _entries[i] with
// { ... }` or a fresh instance) rather than mutating one field in place - the
// readonly claim rules out the copy-and-lose-it bug that a settable field invites.
// Next threads two different chains depending on context - the bucket's chain when
// the slot is live, the free list when it isn't - never both at once.
internal readonly record struct HashMapEntry<TKey, TValue>(int HashCode, int Next, TKey Key, TValue Value);
