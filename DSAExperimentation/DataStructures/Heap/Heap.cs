
namespace DSAExperimentation.DataStructures.Heap;

// Push/Pop are O(log n) - a claim that assumes HeapArray<Element>.Get/Set/Swap are O(1). See
// ARCHITECTURE.md §8 and HeapArray.cs: that assumption holds today but is a Representation-layer
// property this type depends on, not something the type system enforces.
internal sealed class Heap<Element, TOrder>
    where TOrder : struct, IHeapOrder<Element>
{
    private const string EmptyHeapMessage = "The heap contains no elements.";

    private readonly HeapArray<Element> _store = new();

    public int Count => _store.Count;

    public void Push(Element item)
    {
        _store.Add(item);
        SiftUp(_store.Count - 1);
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

    public Element Peek() => TryPeek(out var item) ? item : ThrowEmptyHeap();

    public bool TryPeek(out Element item)
    {
        if (Count == 0)
        {
            // presumption: allow -- item is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            item = default!;
            return false;
        }

        item = _store.Get(0);
        return true;
    }

    public Element Pop()
    {
        if (Count == 0)
        {
            return ThrowEmptyHeap();
        }

        var root = _store.Get(0);
        RemoveRootAndRestoreHeapProperty();
        return root;
    }

    private void RemoveRootAndRestoreHeapProperty()
    {
        var lastIndex = _store.Count - 1;

        _store.Swap(0, lastIndex);
        _store.RemoveLast();

        if (_store.Count > 0)
        {
            SiftDown(0);
        }
    }

    private void SiftDown(int index)
    {
        // Stops when HighestPriorityChild reports index itself: no child outranks
        // the current node, so the heap property already holds below it.
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

    public bool TryPop(out Element item)
    {
        if (Count == 0)
        {
            // presumption: allow -- item is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            item = default!;
            return false;
        }

        item = Pop();
        return true;
    }

    private static Element ThrowEmptyHeap() => throw new InvalidOperationException(EmptyHeapMessage);
}
