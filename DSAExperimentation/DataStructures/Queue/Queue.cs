using DSAExperimentation.DataStructures.Deque;

namespace DSAExperimentation.DataStructures.Queue;

// FIFO is Deque restricted to one end each: Enqueue only ever pushes the back,
// Dequeue only ever pops the front - the same "sequence + access constraint"
// relationship Stack has with DynamicArray, just composing Deque's wraparound
// representation instead of DynamicArray's flat one.
internal sealed class Queue<Element>
{
    private const string EmptyQueueMessage = "The queue contains no elements.";

    private readonly Deque<Element> _items = new();

    public int Count => _items.Count;

    public void Enqueue(Element item) => _items.PushBack(item);

    public Element Peek()
        => Count == 0
            ? ThrowEmptyQueue()
            : _items.PeekFront();

    public bool TryPeek(out Element item) => _items.TryPeekFront(out item);

    public Element Dequeue()
        => Count == 0
            ? ThrowEmptyQueue()
            : _items.PopFront();

    public bool TryDequeue(out Element item) => _items.TryPopFront(out item);

    private static Element ThrowEmptyQueue() => throw new InvalidOperationException(EmptyQueueMessage);
}
