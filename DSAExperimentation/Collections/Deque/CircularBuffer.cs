namespace DSAExperimentation.Collections.Deque;

// A wraparound array: elements occupy [_head, _head+Count) modulo the buffer's
// length, so both ends can grow without shifting the other end's elements. Growing
// re-linearizes into a fresh array starting at index 0. Callers (Deque) are trusted
// to check Count before calling GetFront/GetBack/RemoveFront/RemoveBack, the same
// split of responsibility Heap has with HeapArray.
internal sealed class CircularBuffer<T>
{
    private T[] _items = new T[ArrayGrowth.InitialCapacity];
    private int _head;

    public int Count { get; private set; }

    public T GetFront() => _items[_head];

    public T GetBack() => _items[BackIndex()];

    public void AddFront(T value)
    {
        EnsureCapacity(Count + 1);
        _head = (_head - 1 + _items.Length) % _items.Length;
        _items[_head] = value;
        Count++;
    }

    public void AddBack(T value)
    {
        EnsureCapacity(Count + 1);
        _items[(_head + Count) % _items.Length] = value;
        Count++;
    }

    public void RemoveFront()
    {
        _items[_head] = default!;
        _head = (_head + 1) % _items.Length;
        Count--;
    }

    public void RemoveBack()
    {
        _items[BackIndex()] = default!;
        Count--;
    }

    private int BackIndex() => (_head + Count - 1) % _items.Length;

    private void EnsureCapacity(int required)
    {
        if (required <= _items.Length)
        {
            return;
        }

        var newCapacity = _items.Length * ArrayGrowth.GrowthFactor;

        if (newCapacity < required)
        {
            newCapacity = required;
        }

        var resized = new T[newCapacity];

        for (var i = 0; i < Count; i++)
        {
            resized[i] = _items[(_head + i) % _items.Length];
        }

        _items = resized;
        _head = 0;
    }
}
