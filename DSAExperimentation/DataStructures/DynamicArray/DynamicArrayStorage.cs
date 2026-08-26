namespace DSAExperimentation.DataStructures.DynamicArray;

// Raw storage only - no bounds checking, no throwing. DynamicArray owns the checked
// Get/Set/Insert/RemoveAt; this type owns the array itself and its growth. A
// manually-doubled Element[] buffer, not a List<Element> wrapper - this is the array
// Representation primitive itself, not a reach for the BCL's own.
internal sealed class DynamicArrayStorage<Element>
{
    private Element[] _items = new Element[ArrayGrowth.InitialCapacity];

    public int Count { get; private set; }

    public Element Get(int index) => _items[index];

    public void Set(int index, Element value) => _items[index] = value;

    public void Add(Element value)
    {
        EnsureCapacity(Count + 1);
        _items[Count] = value;
        Count++;
    }

    // Assumes the caller already validated 0 <= index <= Count.
    public void InsertAt(int index, Element value)
    {
        EnsureCapacity(Count + 1);

        for (var i = Count; i > index; i--)
        {
            _items[i] = _items[i - 1];
        }

        _items[index] = value;
        Count++;
    }

    // Assumes the caller already validated 0 <= index < Count.
    public void RemoveAt(int index)
    {
        for (var i = index; i < Count - 1; i++)
        {
            _items[i] = _items[i + 1];
        }

        Count--;
        // presumption: allow -- the vacated slot is unreachable once Count shrinks
        // past it; this only drops a stale reference for the GC, not a correctness need.
        _items[Count] = default!;
    }

    private void EnsureCapacity(int required)
    {
        if (required <= _items.Length)
        {
            return;
        }

        var newCapacity = Math.Max(_items.Length * ArrayGrowth.GrowthFactor, required);

        var resized = new Element[newCapacity];
        Array.Copy(_items, resized, Count);
        _items = resized;
    }
}
