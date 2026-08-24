namespace DSAExperimentation.Collections.Deque;

internal sealed class Deque<T>
{
    private const string EmptyDequeMessage = "The deque contains no elements.";

    private readonly CircularBuffer<T> _items = new();

    public int Count => _items.Count;

    public void PushFront(T item) => _items.AddFront(item);

    public void PushBack(T item) => _items.AddBack(item);

    public T PeekFront()
        => Count == 0
            ? ThrowEmptyDeque()
            : _items.GetFront();

    public T PeekBack()
        => Count == 0
            ? ThrowEmptyDeque()
            : _items.GetBack();

    public bool TryPeekFront(out T item)
    {
        if (Count == 0)
        {
            item = default!;
            return false;
        }

        item = _items.GetFront();
        return true;
    }

    public bool TryPeekBack(out T item)
    {
        if (Count == 0)
        {
            item = default!;
            return false;
        }

        item = _items.GetBack();
        return true;
    }

    public T PopFront()
    {
        var item = PeekFront();
        _items.RemoveFront();
        return item;
    }

    public T PopBack()
    {
        var item = PeekBack();
        _items.RemoveBack();
        return item;
    }

    public bool TryPopFront(out T item)
    {
        if (!TryPeekFront(out item))
        {
            return false;
        }

        _items.RemoveFront();
        return true;
    }

    public bool TryPopBack(out T item)
    {
        if (!TryPeekBack(out item))
        {
            return false;
        }

        _items.RemoveBack();
        return true;
    }

    private static T ThrowEmptyDeque() => throw new InvalidOperationException(EmptyDequeMessage);
}
