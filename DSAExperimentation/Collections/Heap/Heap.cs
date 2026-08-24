namespace DSAExperimentation.Collections.Heap;

// Push/Pop are O(log n) - a claim that assumes HeapArray<T>.Get/Set/Swap are O(1). See
// ARCHITECTURE.md §8 and HeapArray.cs: that assumption holds today but is a Representation-layer
// property this type depends on, not something the type system enforces.
internal sealed class Heap<T, TOrder>
    where TOrder : struct, IHeapOrder<T>
{
    private const string EmptyHeapMessage = "The heap contains no elements.";

    private readonly HeapArray<T> _store = new();

    public int Count => _store.Count;

    public void Push(T item)
    {
        _store.Add(item);
        SiftUp(_store.Count - 1);
    }

    public T Peek()
        => Count == 0
            ? ThrowEmptyHeap()
            : _store.Get(0);

    public bool TryPeek(out T item)
    {
        if (Count == 0)
        {
            item = default!;
            return false;
        }

        item = _store.Get(0);
        return true;
    }

    public T Pop()
    {
        if (Count == 0)
        {
            return ThrowEmptyHeap();
        }

        var root = _store.Get(0);
        var lastIndex = _store.Count - 1;

        _store.Swap(0, lastIndex);
        _store.RemoveLast();

        if (_store.Count > 0)
        {
            SiftDown(0);
        }

        return root;
    }

    public bool TryPop(out T item)
    {
        if (Count == 0)
        {
            item = default!;
            return false;
        }

        item = Pop();
        return true;
    }

    private void SiftUp(int index)
    {
        while (index > 0)
        {
            var parent = HeapArrayIndex.Parent(index);

            if (!TOrder.HasPriority(_store.Get(index), _store.Get(parent)))
            {
                return;
            }

            _store.Swap(index, parent);
            index = parent;
        }
    }

    private void SiftDown(int index)
    {
        while (true)
        {
            var highestPriority = HighestPriorityChild(index);

            if (highestPriority == index)
            {
                return;
            }

            _store.Swap(index, highestPriority);
            index = highestPriority;
        }
    }

    private int HighestPriorityChild(int index)
    {
        var highestPriority = index;
        var left = HeapArrayIndex.LeftChild(index);
        var right = HeapArrayIndex.RightChild(index);

        if (left < _store.Count && TOrder.HasPriority(_store.Get(left), _store.Get(highestPriority)))
        {
            highestPriority = left;
        }

        if (right < _store.Count && TOrder.HasPriority(_store.Get(right), _store.Get(highestPriority)))
        {
            highestPriority = right;
        }

        return highestPriority;
    }

    private static T ThrowEmptyHeap() => throw new InvalidOperationException(EmptyHeapMessage);
}
