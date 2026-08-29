
namespace DSAExperimentation.DataStructures.DynamicArray;

internal sealed class DynamicArray<Element>
{
    private const string IndexOutOfRangeMessage = "Index was outside the bounds of the array.";

    private readonly DynamicArrayStorage<Element> _storage = new();

    public int Count => _storage.Count;

    // Throwing convenience beside each TryX's recoverable form. Unlike Heap/Stack/
    // Deque/Queue/HashMap - whose Peek/Pop/Get once had a throwing form too, since
    // removed - this one stays: an out-of-range index is Stack.cs's and Sequence's
    // own internal-trusted callers' bug, not state an external caller can't already
    // know (see ARCHITECTURE.md's Stack/Sequence sections), so throwing loudly here
    // beats silently discarding a TryGet result at every internal call site.
    public Element Get(int index)
        => TryGet(index, out var value) ? value : ThrowIndexOutOfRange(index);

    public void Set(int index, Element value)
    {
        if (!TrySet(index, value))
        {
            ThrowIndexOutOfRange(index);
        }
    }

    public void Add(Element value) => _storage.Add(value);

    public void Insert(int index, Element value)
    {
        if (!TryInsert(index, value))
        {
            ThrowIndexOutOfRange(index);
        }
    }

    public void RemoveAt(int index)
    {
        if (!TryRemoveAt(index))
        {
            ThrowIndexOutOfRange(index);
        }
    }

    public bool TryGet(int index, out Element value)
    {
        if (!IsValidIndex(index))
        {
            // presumption: allow -- value is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            value = default!;
            return false;
        }

        value = _storage.Get(index);
        return true;
    }

    public bool TrySet(int index, Element value)
    {
        if (!IsValidIndex(index))
        {
            return false;
        }

        _storage.Set(index, value);
        return true;
    }

    public bool TryInsert(int index, Element value)
    {
        if (!IsValidInsertIndex(index))
        {
            return false;
        }

        _storage.InsertAt(index, value);
        return true;
    }

    private bool IsValidInsertIndex(int index) => index >= 0 && index <= Count;

    public bool TryRemoveAt(int index)
    {
        if (!IsValidIndex(index))
        {
            return false;
        }

        _storage.RemoveAt(index);
        return true;
    }

    private bool IsValidIndex(int index) => index >= 0 && index < Count;

    private static Element ThrowIndexOutOfRange(int index)
        => throw new ArgumentOutOfRangeException(nameof(index), IndexOutOfRangeMessage);
}
