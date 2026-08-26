using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.DataStructures.Stack;

// A stack isn't a distinct physical structure - it's a sequence plus a LIFO access
// constraint, so it composes DynamicArray directly rather than managing its own
// buffer. RemoveAt(Count-1) is O(1): the shift loop inside it never executes when
// the removed index is already the last one.
internal sealed class Stack<Element>
{
    private const string EmptyStackMessage = "The stack contains no elements.";

    private readonly DynamicArray<Element> _items = new();

    public int Count => _items.Count;

    public void Push(Element item) => _items.Add(item);

    public Element Peek() => TryPeek(out var item) ? item : ThrowEmptyStack();

    private static Element ThrowEmptyStack() => throw new InvalidOperationException(EmptyStackMessage);

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

    public Element Pop()
    {
        var item = Peek();
        _items.RemoveAt(Count - 1);
        return item;
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
