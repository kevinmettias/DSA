namespace DSAExperimentation.DataStructures.CircularBuffer;

// The invariant a CircularBuffer configuration injects: how many elements it may hold. A push the
// capacity has no room for is refused, never made room for - dropping an element to admit another
// would be eviction, a mechanism's rule, not a data structure's (ARCHITECTURE 17.6b).
//
// The set is closed to the two capacities this library enumerates, unbounded and bounded at k, like
// IHeapOrder's two orders (ARCHITECTURE 9.2). Unlike them, a bound carries a number only known at run
// time, so the witness is a struct instance the buffer holds in a field rather than a static abstract
// member: the JIT still specializes CircularBuffer per witness type, and UnboundedCapacity's constant
// answer folds every capacity check away (ARCHITECTURE 20).
/// <summary>
/// How many elements a <see cref="CircularBuffer{Element, TCapacity}"/> may hold: whether it is full at a given count.
/// </summary>
public interface IBufferCapacity
{
    /// <summary>Reports whether a buffer holding <paramref name="count"/> elements has no room for another.</summary>
    /// <param name="count">The number of elements the buffer holds.</param>
    /// <returns>true when one more element would exceed the capacity; otherwise false.</returns>
    bool IsFullAt(int count);
}
