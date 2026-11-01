using DSAExperimentation.DataStructures.Deque;

namespace DSAExperimentation.DataStructures.MonotonicDeque;

// The extremum of a sliding window in amortized O(1) per step: positions in push order, each with its
// key, keeping only the entries no later arrival has dominated (see IWindowOrder). The front is the
// window's extremum; Push evicts from the back what the arriving key dominates; EvictBefore drops from
// the front what has slid out. Fifteen solutions wrote this by hand over Deque<int>, reading each key
// back out of an array by index; storing the key beside its position covers the ones whose key is
// computed as they go (dp[i], y - x, dp - prefix) and so has no array to read back.
//
// Precondition law, unchecked: positions are pushed in non-decreasing order. That is what makes the
// front the oldest entry, so that EvictBefore can stop at the first position still inside the window.
// A position is whatever the window is measured in - an index for a fixed window of k, an x-coordinate
// for a window of width d.
//
// Composes Deque the way Queue and Stack compose the structure each restricts, in a folder of its own
// beside it (§13.5). Complexity law (§8): each entry is pushed once and popped at most once, so Push is
// amortized O(1) only because Deque's operations at both ends are O(1) through its CircularBuffer.
/// <summary>
/// A deque of (position, key) entries whose front is a sliding window's extremum, in amortized O(1) per step.
/// </summary>
public sealed class MonotonicDeque<Key, TOrder>
    where TOrder : struct, IWindowOrder<Key>
{
    private readonly Deque<(int Position, Key Key)> _entries = new();

    /// <summary>Appends an entry at the back after evicting every entry whose key it dominates.</summary>
    /// <param name="position">The entry's position, which must not be below any position already pushed.</param>
    /// <param name="key">The entry's key.</param>
    public void Push(int position, Key key)
    {
        while (_entries.TryPeekBack(out var back) && TOrder.IsDominatedBy(back.Key, key))
        {
            _entries.TryPopBack(out _);
        }

        _entries.PushBack((position, key));
    }

    // Drops every front entry whose position is below firstPosition - the ones the window has slid past.
    /// <summary>Drops every entry whose position is below <paramref name="firstPosition"/>.</summary>
    /// <param name="firstPosition">The first position still inside the window.</param>
    public void EvictBefore(int firstPosition)
    {
        while (_entries.TryPeekFront(out var front) && front.Position < firstPosition)
        {
            _entries.TryPopFront(out _);
        }
    }

    /// <summary>Reads the front entry, the extremum of the keys still held, without removing it.</summary>
    /// <param name="front">The front entry, newest on a tie, or <c>default</c> when the deque is empty.</param>
    /// <returns>true when the deque holds an entry; false when it is empty.</returns>
    public bool TryPeekFront(out (int Position, Key Key) front) => _entries.TryPeekFront(out front);

    /// <summary>Removes the front entry, the extremum of the keys still held.</summary>
    /// <param name="front">The removed entry, newest on a tie, or <c>default</c> when the deque is empty.</param>
    /// <returns>true when an entry was removed; false when the deque is empty.</returns>
    public bool TryPopFront(out (int Position, Key Key) front) => _entries.TryPopFront(out front);
}
