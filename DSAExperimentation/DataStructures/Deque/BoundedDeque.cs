using DSAExperimentation.DataStructures.CircularBuffer;

namespace DSAExperimentation.DataStructures.Deque;

// CircularBuffer bounded at a fixed capacity (ARCHITECTURE 20), derived rather than wrapped, as Deque
// is: the buffer's whole surface, with each push a Try that a full deque refuses rather than grows
// for. Refusing is all the bound ever does - nothing held is dropped to make room, which would make
// this a mechanism (ARCHITECTURE 17.6b).
/// <summary>A double-ended queue that holds at most a fixed number of elements, refusing a push when full.</summary>
/// <param name="capacity">The most elements the deque may hold, at least 1.</param>
/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is below 1.</exception>
public sealed class BoundedDeque<Element>(int capacity) : CircularBuffer<Element, BoundedCapacity>(new BoundedCapacity(capacity));
