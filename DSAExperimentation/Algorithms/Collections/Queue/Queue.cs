using DSAExperimentation.Collections.Deque;

namespace DSAExperimentation.Algorithms.Collections.Queue;

// FIFO is Deque restricted to one end each: Enqueue only ever pushes the back,
// Dequeue only ever pops the front - the same "sequence + access constraint"
// relationship Stack has with DynamicArray, just composing Deque's wraparound
// representation instead of DynamicArray's flat one.
internal sealed class Queue<T>
{
    private const string EmptyQueueMessage = "The queue contains no elements.";

    private readonly Deque<T> _items = new();

    public int Count => _items.Count;

    public void Enqueue(T item) => _items.PushBack(item);

    public T Peek()
        => Count == 0
            ? ThrowEmptyQueue()
            : _items.PeekFront();

    public bool TryPeek(out T item) => _items.TryPeekFront(out item);

    public T Dequeue()
    {
        if (Count == 0)
        {
            return ThrowEmptyQueue();
        }

        return _items.PopFront();
    }

    public bool TryDequeue(out T item) => _items.TryPopFront(out item);

    private static T ThrowEmptyQueue() => throw new InvalidOperationException(EmptyQueueMessage);
}
