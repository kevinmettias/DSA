using DSAExperimentation.DataStructures.Deque;

namespace DSAExperimentation.DataStructures.Queue;

// FIFO is Deque restricted to one end each: Enqueue only ever pushes the back,
// Dequeue only ever pops the front - the same "sequence + access constraint"
// relationship Stack has with DynamicArray, just composing Deque's wraparound
// representation instead of DynamicArray's flat one.
//
// No throwing Peek/Dequeue convenience: whether the queue is empty is state an
// external caller can't always know in advance, so TryPeek/TryDequeue are the only
// public surface for it, forcing callers onto the return-value form instead of a
// try/catch.
/// <summary>A first-in, first-out queue.</summary>
public sealed class Queue<Element>
{
    private readonly Deque<Element> _items = new();

    /// <summary>The number of elements in the queue.</summary>
    public int Count => _items.Count;

    /// <summary>Adds an element at the back of the queue.</summary>
    /// <param name="item">The element to add.</param>
    public void Enqueue(Element item) => _items.PushBack(item);

    /// <summary>Reads the front element, the oldest one still queued, without removing it.</summary>
    /// <param name="item">The front element, or <c>default</c> when the queue is empty.</param>
    /// <returns>true when the queue holds an element; false when it is empty.</returns>
    public bool TryPeek(out Element item) => _items.TryPeekFront(out item);

    /// <summary>Removes the front element, the oldest one still queued.</summary>
    /// <param name="item">The removed element, or <c>default</c> when the queue is empty.</param>
    /// <returns>true when an element was removed; false when the queue is empty.</returns>
    public bool TryDequeue(out Element item) => _items.TryPopFront(out item);
}
