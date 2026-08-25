
namespace DSAExperimentation.DataStructures.DynamicArray;

internal sealed class DynamicArray<T>
{
    private const string IndexOutOfRangeMessage = "Index was outside the bounds of the array.";

    private readonly DynamicArrayStorage<T> _storage = new();

    public int Count => _storage.Count;

    public T Get(int index)
    {
        ValidateIndex(index);
        return _storage.Get(index);
    }

    public void Set(int index, T value)
    {
        ValidateIndex(index);
        _storage.Set(index, value);
    }

    public void Add(T value) => _storage.Add(value);

    public void Insert(int index, T value)
    {
        if (index < 0 || index > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), IndexOutOfRangeMessage);
        }

        _storage.InsertAt(index, value);
    }

    public void RemoveAt(int index)
    {
        ValidateIndex(index);
        _storage.RemoveAt(index);
    }

    private void ValidateIndex(int index)
    {
        if (index < 0 || index >= Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), IndexOutOfRangeMessage);
        }
    }
}
