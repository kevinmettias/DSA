namespace DSAExperimentation.DataStructures.DynamicArray;

// Raw storage only - no bounds checking, no throwing. DynamicArray owns the checked
// Get/Set/Insert/RemoveAt; this type owns the array itself and its growth. A
// manually-doubled T[] buffer, not a List<T> wrapper - this is the array
// Representation primitive itself, not a reach for the BCL's own.
internal sealed class DynamicArrayStorage<T>
{
    private T[] _items = new T[ArrayGrowth.InitialCapacity];

    public int Count { get; private set; }

    public T Get(int index) => _items[index];

    public void Set(int index, T value) => _items[index] = value;

    public void Add(T value)
    {
        EnsureCapacity(Count + 1);
        _items[Count] = value;
        Count++;
    }

    // Assumes the caller already validated 0 <= index <= Count.
    public void InsertAt(int index, T value)
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
        _items[Count] = default!;
    }

    private void EnsureCapacity(int required)
    {
        if (required <= _items.Length)
        {
            return;
        }

        var newCapacity = _items.Length * ArrayGrowth.GrowthFactor;

        if (newCapacity < required)
        {
            newCapacity = required;
        }

        var resized = new T[newCapacity];
        Array.Copy(_items, resized, Count);
        _items = resized;
    }
}
