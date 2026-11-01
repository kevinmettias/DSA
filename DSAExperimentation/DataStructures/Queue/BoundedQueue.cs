using DSAExperimentation.DataStructures.CircularBuffer;

namespace DSAExperimentation.DataStructures.Queue;

// Queue bounded at a fixed capacity (ARCHITECTURE 20): FIFO's one end each, over CircularBuffer
// bounded rather than unbounded, so Enqueue becomes a Try that a full queue refuses. Refusing is all
// the bound ever does - nothing queued is dropped to make room, which would make this a mechanism
// (ARCHITECTURE 17.6b).
/// <summary>A first-in, first-out queue that holds at most a fixed number of elements, refusing to enqueue when full.</summary>
public sealed class BoundedQueue<Element>
{
    private readonly CircularBuffer<Element, BoundedCapacity> _items;

    /// <summary>The number of elements in the queue.</summary>
    public int Count => _items.Count;

    /// <summary>Whether the queue holds as many elements as its capacity allows, so that enqueueing is refused.</summary>
    public bool IsFull => _items.IsFull;

    /// <summary>Creates an empty queue that holds at most <paramref name="capacity"/> elements.</summary>
    /// <param name="capacity">The most elements the queue may hold, at least 1.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is below 1.</exception>
    public BoundedQueue(int capacity) => _items = new(new BoundedCapacity(capacity));

    /// <summary>Adds an element at the back of the queue, unless it is full.</summary>
    /// <param name="item">The element to add.</param>
    /// <returns>true when the element was added; false, changing nothing, when the queue is full.</returns>
    public bool TryEnqueue(Element item) => _items.TryPushBack(item);

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
