namespace DSAExperimentation.Collections.DynamicArray;

// A manually-doubled T[] buffer, not a List<T> wrapper - this is the array
// Representation primitive itself, not a reach for the BCL's own. Stack composes
// this directly (see Collections/Stack) instead of duplicating growth logic.
internal sealed class DynamicArray<T>
{
    private const string IndexOutOfRangeMessage = "Index was outside the bounds of the array.";

    private T[] _items = new T[ArrayGrowth.InitialCapacity];

    public int Count { get; private set; }

    public T Get(int index)
    {
        ValidateIndex(index);
        return _items[index];
    }

    public void Set(int index, T value)
    {
        ValidateIndex(index);
        _items[index] = value;
    }

    public void Add(T value)
    {
        EnsureCapacity(Count + 1);
        _items[Count] = value;
        Count++;
    }

    public void Insert(int index, T value)
    {
        if (index < 0 || index > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), IndexOutOfRangeMessage);
        }

        EnsureCapacity(Count + 1);

        for (var i = Count; i > index; i--)
        {
            _items[i] = _items[i - 1];
        }

        _items[index] = value;
        Count++;
    }

    public void RemoveAt(int index)
    {
        ValidateIndex(index);

        for (var i = index; i < Count - 1; i++)
        {
            _items[i] = _items[i + 1];
        }

        Count--;
        _items[Count] = default!;
    }

    private void ValidateIndex(int index)
    {
        if (index < 0 || index >= Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), IndexOutOfRangeMessage);
        }
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
