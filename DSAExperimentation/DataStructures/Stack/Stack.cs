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
internal sealed class Stack<Element>
{
    private readonly DynamicArray<Element> _items = new();

    public int Count => _items.Count;

    public void Push(Element item) => _items.Add(item);

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
