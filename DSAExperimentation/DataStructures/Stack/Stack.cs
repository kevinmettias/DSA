using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.DataStructures.Stack;

// A stack isn't a distinct physical structure - it's a sequence plus a LIFO access
// constraint, so it composes DynamicArray directly rather than managing its own
// buffer. RemoveAt(Count-1) is O(1): the shift loop inside it never executes when
// the removed index is already the last one.
//
// No throwing Peek/Pop convenience: whether the stack is empty is state an external
// caller can't always know in advance, so TryPeek/TryPop are the only public surface
// for it, forcing callers onto the return-value form instead of a try/catch.
/// <summary>A last-in, first-out stack: amortized constant-time push, constant-time peek and pop.</summary>
public sealed class Stack<Element>
{
    private readonly DynamicArray<Element> _items = new();

    /// <summary>The number of elements on the stack.</summary>
    public int Count => _items.Count;

    /// <summary>Puts an element on top of the stack.</summary>
    /// <param name="item">The element to push.</param>
    public void Push(Element item) => _items.Add(item);

    /// <summary>Reads the top element without removing it.</summary>
    /// <param name="item">The top element when the stack is not empty; otherwise <c>default</c>.</param>
    /// <returns>True when there is a top element to read; false when the stack is empty.</returns>
    public bool TryPeek(out Element item)
    {
        if (Count == 0)
        {
            // presumption: allow -- item is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            item = default!;
            return false;
        }

        item = _items.Get(Count - 1);
        return true;
    }

    /// <summary>Removes the top element and hands it back.</summary>
    /// <param name="item">The removed element when the stack was not empty; otherwise <c>default</c>.</param>
    /// <returns>True when an element was removed; false when the stack was empty.</returns>
    public bool TryPop(out Element item)
    {
        if (!TryPeek(out item))
        {
            return false;
        }

        _items.RemoveAt(Count - 1);
        return true;
    }
}
