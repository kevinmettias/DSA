
namespace DSAExperimentation.DataStructures.Deque;

// No throwing PeekFront/PeekBack/PopFront/PopBack convenience: whether the deque is
// empty is state an external caller can't always know in advance, so the TryX forms
// are the only public surface for it, forcing callers onto the return-value form
// instead of a try/catch.
/// <summary>A double-ended queue that adds, reads and removes elements at either end.</summary>
public sealed class Deque<Element>
{
    private readonly CircularBuffer<Element> _items = new();

    /// <summary>The number of elements in the deque.</summary>
    public int Count => _items.Count;

    /// <summary>Adds an element at the front.</summary>
    /// <param name="item">The element to add.</param>
    public void PushFront(Element item) => _items.AddFront(item);

    /// <summary>Adds an element at the back.</summary>
    /// <param name="item">The element to add.</param>
    public void PushBack(Element item) => _items.AddBack(item);

    /// <summary>Reads the element at the front without removing it.</summary>
    /// <param name="item">The front element when the deque is not empty; otherwise the default value.</param>
    /// <returns>True when the deque holds an element; false when it is empty.</returns>
    public bool TryPeekFront(out Element item)
    {
        if (Count == 0)
        {
            // presumption: allow -- item is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            item = default!;
            return false;
        }

        item = _items.GetFront();
        return true;
    }

    /// <summary>Reads the element at the back without removing it.</summary>
    /// <param name="item">The back element when the deque is not empty; otherwise the default value.</param>
    /// <returns>True when the deque holds an element; false when it is empty.</returns>
    public bool TryPeekBack(out Element item)
    {
        if (Count == 0)
        {
            // presumption: allow -- item is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            item = default!;
            return false;
        }

        item = _items.GetBack();
        return true;
    }

    /// <summary>Removes the element at the front.</summary>
    /// <param name="item">The removed element when the deque was not empty; otherwise the default value.</param>
    /// <returns>True when an element was removed; false, changing nothing, when the deque is empty.</returns>
    public bool TryPopFront(out Element item)
    {
        if (!TryPeekFront(out item))
        {
            return false;
        }

        _items.RemoveFront();
        return true;
    }

    /// <summary>Removes the element at the back.</summary>
    /// <param name="item">The removed element when the deque was not empty; otherwise the default value.</param>
    /// <returns>True when an element was removed; false, changing nothing, when the deque is empty.</returns>
    public bool TryPopBack(out Element item)
    {
        if (!TryPeekBack(out item))
        {
            return false;
        }

        _items.RemoveBack();
        return true;
    }
}
