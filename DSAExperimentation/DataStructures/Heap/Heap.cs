
namespace DSAExperimentation.DataStructures.Heap;

// Push/Pop are O(log n) - a claim that assumes HeapArray<Element>.Get/Set/Swap are O(1). See
// ARCHITECTURE.md §8 and HeapArray.cs: that assumption holds today but is a Representation-layer
// property this type depends on, not something the type system enforces.
//
// No throwing Peek/Pop convenience: whether the heap is empty is state an external
// caller can't always know in advance, so TryPeek/TryPop are the only public surface
// for it, forcing callers onto the return-value form instead of a try/catch.
/// <summary>A binary heap that yields its highest-priority element first, ranked by <c>TOrder</c>.</summary>
public sealed class Heap<Element, TOrder>
    where TOrder : struct, IHeapOrder<Element>
{
    private readonly HeapArray<Element> _store = new();

    /// <summary>The number of elements in the heap.</summary>
    public int Count => _store.Count;

    /// <summary>Adds an element to the heap in O(log n).</summary>
    /// <param name="item">The element to add.</param>
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

    /// <summary>Reads the highest-priority element without removing it.</summary>
    /// <param name="item">The highest-priority element, or <c>default</c> when the heap is empty.</param>
    /// <returns>true when the heap holds an element; false when it is empty.</returns>
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

    /// <summary>Removes the highest-priority element in O(log n).</summary>
    /// <param name="item">The removed element, or <c>default</c> when the heap is empty.</param>
    /// <returns>true when an element was removed; false when the heap is empty.</returns>
    public bool TryPop(out Element item)
    {
        if (!TryPeek(out item))
        {
            return false;
        }

        RemoveRootAndRestoreHeapProperty();
        return true;
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
}
