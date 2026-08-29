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
internal sealed class Queue<Element>
{
    private readonly Deque<Element> _items = new();

    public int Count => _items.Count;

    public void Enqueue(Element item) => _items.PushBack(item);

    public bool TryPeek(out Element item) => _items.TryPeekFront(out item);

    public bool TryDequeue(out Element item) => _items.TryPopFront(out item);
}
