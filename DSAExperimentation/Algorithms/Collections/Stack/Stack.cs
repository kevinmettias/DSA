using DSAExperimentation.Collections.DynamicArray;

namespace DSAExperimentation.Algorithms.Collections.Stack;

// A stack isn't a distinct physical structure - it's a sequence plus a LIFO access
// constraint, so it composes DynamicArray directly rather than managing its own
// buffer. RemoveAt(Count-1) is O(1): the shift loop inside it never executes when
// the removed index is already the last one.
internal sealed class Stack<T>
{
    private const string EmptyStackMessage = "The stack contains no elements.";

    private readonly DynamicArray<T> _items = new();

    public int Count => _items.Count;

    public void Push(T item) => _items.Add(item);

    public T Peek()
        => Count == 0
            ? ThrowEmptyStack()
            : _items.Get(Count - 1);

    public bool TryPeek(out T item)
    {
        if (Count == 0)
        {
            item = default!;
            return false;
        }

        item = _items.Get(Count - 1);
        return true;
    }

    public T Pop()
    {
        var item = Peek();
        _items.RemoveAt(Count - 1);
        return item;
    }

    public bool TryPop(out T item)
    {
        if (!TryPeek(out item))
        {
            return false;
        }

        _items.RemoveAt(Count - 1);
        return true;
    }

    private static T ThrowEmptyStack() => throw new InvalidOperationException(EmptyStackMessage);
}
