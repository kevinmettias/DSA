using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.DataStructures.CursorSequence;

// A sequence with a cursor sitting between two of its elements, edited and read at the cursor. It is
// the composition Stack makes over DynamicArray (ARCHITECTURE.md §4.1), with a cursor in place of the
// LIFO constraint: two DynamicArrays, the elements before the cursor in order - so the last of them
// sits immediately left of it - and the elements after it reversed, so their last sits immediately
// right. An edit at the cursor costs O(1), a cursor move one transfer per element it crosses, and any
// element before the cursor reads by index in O(1).
//
// A data structure under §17.6b's line, not a mechanism: an element enters only through
// InsertBeforeCursor and leaves only through RemoveBeforeCursor or ClearAfterCursor, each naming what
// goes by its position. The cursor's bounds clamp a move, the way an empty stack refuses a pop.
/// <summary>A sequence with a cursor between its elements, edited at the cursor and read to its left.</summary>
public sealed class CursorSequence<Element>
{
    private readonly DynamicArray<Element> _before = new();
    private readonly DynamicArray<Element> _afterReversed = new();

    /// <summary>The number of elements before the cursor, which is the cursor's position.</summary>
    public int CountBeforeCursor => _before.Count;

    /// <summary>The number of elements after the cursor.</summary>
    public int CountAfterCursor => _afterReversed.Count;

    /// <summary>Inserts an element immediately before the cursor, which stays just after it.</summary>
    /// <param name="item">The element to insert.</param>
    public void InsertBeforeCursor(Element item) => _before.Add(item);

    /// <summary>Removes up to a number of the elements immediately before the cursor.</summary>
    /// <param name="maxCount">The most elements to remove, at least 0.</param>
    /// <returns>How many were removed: fewer than asked when fewer precede the cursor.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxCount"/> is negative.</exception>
    public int RemoveBeforeCursor(int maxCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxCount);

        var removed = Math.Min(maxCount, _before.Count);

        for (var i = 0; i < removed; i++)
        {
            _before.RemoveAt(_before.Count - 1);
        }

        return removed;
    }

    /// <summary>Moves the cursor left across up to a number of elements, stopping at the start.</summary>
    /// <param name="maxSteps">The most elements to cross, at least 0.</param>
    /// <returns>How many elements the cursor crossed.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxSteps"/> is negative.</exception>
    public int MoveLeft(int maxSteps) => Transfer(_before, _afterReversed, maxSteps);

    /// <summary>Moves the cursor right across up to a number of elements, stopping at the end.</summary>
    /// <param name="maxSteps">The most elements to cross, at least 0.</param>
    /// <returns>How many elements the cursor crossed.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxSteps"/> is negative.</exception>
    public int MoveRight(int maxSteps) => Transfer(_afterReversed, _before, maxSteps);

    /// <summary>Removes every element after the cursor.</summary>
    public void ClearAfterCursor()
    {
        while (_afterReversed.Count > 0)
        {
            _afterReversed.RemoveAt(_afterReversed.Count - 1);
        }
    }

    /// <summary>Reads an element before the cursor by its position from the start of the sequence.</summary>
    /// <param name="index">A 0-based position from 0 to <see cref="CountBeforeCursor"/> - 1.</param>
    /// <returns>The element at that position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The position is not before the cursor.</exception>
    public Element GetBeforeCursor(int index) => _before.Get(index);

    // Moves up to maxSteps elements from the end of one side to the end of the other: crossing an
    // element moves it from immediately one side of the cursor to immediately the other.
    private static int Transfer(DynamicArray<Element> from, DynamicArray<Element> to, int maxSteps)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxSteps);

        var moved = Math.Min(maxSteps, from.Count);

        for (var i = 0; i < moved; i++)
        {
            var last = from.Count - 1;
            to.Add(from.Get(last));
            from.RemoveAt(last);
        }

        return moved;
    }
}
