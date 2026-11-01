
namespace DSAExperimentation.DataStructures.DynamicArray;

/// <summary>
/// A growable array addressed by 0-based index, offering each bounds-checked operation in a throwing and a Try form.
/// </summary>
public sealed class DynamicArray<Element>
{
    private const string IndexOutOfRangeMessage = "Index was outside the bounds of the array.";

    private readonly DynamicArrayStorage<Element> _storage = new();

    /// <summary>The number of elements in the array.</summary>
    public int Count => _storage.Count;

    // Throwing convenience beside each TryX's recoverable form. Unlike Heap/Stack/
    // Deque/Queue/HashMap - whose Peek/Pop/Get once had a throwing form too, since
    // removed - this one stays: an out-of-range index is Stack.cs's and Sequence's
    // own internal-trusted callers' bug, not state an external caller can't already
    // know (see ARCHITECTURE.md's Stack/Sequence sections), so throwing loudly here
    // beats silently discarding a TryGet result at every internal call site.
    /// <summary>Reads the element at an index.</summary>
    /// <param name="index">A 0-based index from 0 to <see cref="Count"/> - 1.</param>
    /// <returns>The element at the index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The index is negative or not below <see cref="Count"/>.
    /// </exception>
    public Element Get(int index)
        => TryGet(index, out var value) ? value : ThrowIndexOutOfRange(index);

    /// <summary>Replaces the element at an index.</summary>
    /// <param name="index">A 0-based index from 0 to <see cref="Count"/> - 1.</param>
    /// <param name="value">The element to store there.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The index is negative or not below <see cref="Count"/>.
    /// </exception>
    public void Set(int index, Element value)
    {
        if (!TrySet(index, value))
        {
            ThrowIndexOutOfRange(index);
        }
    }

    /// <summary>Appends an element at the end.</summary>
    /// <param name="value">The element to append.</param>
    public void Add(Element value) => _storage.Add(value);

    /// <summary>
    /// Inserts an element at an index, shifting the elements at and after it one place toward the end.
    /// </summary>
    /// <param name="index">A 0-based index from 0 to <see cref="Count"/> inclusive.</param>
    /// <param name="value">The element to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The index is negative or greater than <see cref="Count"/>.
    /// </exception>
    public void Insert(int index, Element value)
    {
        if (!TryInsert(index, value))
        {
            ThrowIndexOutOfRange(index);
        }
    }

    /// <summary>
    /// Removes the element at an index, shifting the elements after it one place toward the front.
    /// </summary>
    /// <param name="index">A 0-based index from 0 to <see cref="Count"/> - 1.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The index is negative or not below <see cref="Count"/>.
    /// </exception>
    public void RemoveAt(int index)
    {
        if (!TryRemoveAt(index))
        {
            ThrowIndexOutOfRange(index);
        }
    }

    /// <summary>Reads the element at an index if the index is valid.</summary>
    /// <param name="index">The 0-based index to read, valid from 0 to <see cref="Count"/> - 1.</param>
    /// <param name="value">The element at the index when it is valid; otherwise the default value.</param>
    /// <returns>True when the index is valid; false otherwise.</returns>
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

    /// <summary>Replaces the element at an index if the index is valid.</summary>
    /// <param name="index">The 0-based index to write, valid from 0 to <see cref="Count"/> - 1.</param>
    /// <param name="value">The element to store there.</param>
    /// <returns>True when the element was replaced; false, changing nothing, when the index is not valid.</returns>
    public bool TrySet(int index, Element value)
    {
        if (!IsValidIndex(index))
        {
            return false;
        }

        _storage.Set(index, value);
        return true;
    }

    /// <summary>
    /// Inserts an element at an index if the index is valid, shifting the elements at and after it one place
    /// toward the end.
    /// </summary>
    /// <param name="index">The 0-based index to insert at, valid from 0 to <see cref="Count"/> inclusive.</param>
    /// <param name="value">The element to insert.</param>
    /// <returns>True when the element was inserted; false, changing nothing, when the index is not valid.</returns>
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

    /// <summary>
    /// Removes the element at an index if the index is valid, shifting the elements after it one place toward
    /// the front.
    /// </summary>
    /// <param name="index">The 0-based index of the element to remove, valid from 0 to <see cref="Count"/> - 1.</param>
    /// <returns>True when the element was removed; false, changing nothing, when the index is not valid.</returns>
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
