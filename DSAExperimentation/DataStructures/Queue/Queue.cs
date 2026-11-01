using DSAExperimentation.DataStructures.CircularBuffer;

namespace DSAExperimentation.DataStructures.Queue;

// FIFO is CircularBuffer restricted to one end each (ARCHITECTURE 20): Enqueue only ever pushes the
// back, Dequeue only ever pops the front - the same "sequence + access constraint" relationship
// Stack has with DynamicArray, just over the wraparound representation instead of the flat one.
// Removing the other end's operations is what a facade can do and a type argument cannot. Both
// ends can still be read: the front is the oldest element and the rear the newest, and reading
// either removes nothing.
//
// No throwing Peek/Dequeue convenience: whether the queue is empty is state an
// external caller can't always know in advance, so TryPeek/TryDequeue are the only
// public surface for it, forcing callers onto the return-value form instead of a
// try/catch.
/// <summary>A first-in, first-out queue.</summary>
public sealed class Queue<Element>
{
    private readonly CircularBuffer<Element, UnboundedCapacity> _items = new(default);

    /// <summary>The number of elements in the queue.</summary>
    public int Count => _items.Count;

    /// <summary>Adds an element at the back of the queue.</summary>
    /// <param name="item">The element to add.</param>
    public void Enqueue(Element item) => _ = _items.TryPushBack(item);

    /// <summary>Reads the front element, the oldest one still queued, without removing it.</summary>
    /// <param name="item">The front element, or <c>default</c> when the queue is empty.</param>
    /// <returns>true when the queue holds an element; false when it is empty.</returns>
    public bool TryPeek(out Element item) => _items.TryPeekFront(out item);

    /// <summary>Reads the rear element, the newest one queued, without removing it.</summary>
    /// <param name="item">The rear element, or <c>default</c> when the queue is empty.</param>
    /// <returns>true when the queue holds an element; false when it is empty.</returns>
    public bool TryPeekRear(out Element item) => _items.TryPeekBack(out item);

    /// <summary>Removes the front element, the oldest one still queued.</summary>
    /// <param name="item">The removed element, or <c>default</c> when the queue is empty.</param>
    /// <returns>true when an element was removed; false when the queue is empty.</returns>
    public bool TryDequeue(out Element item) => _items.TryPopFront(out item);
}
