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
internal sealed class MonotonicDeque<Key, TOrder>
    where TOrder : struct, IWindowOrder<Key>
{
    private readonly Deque<(int Position, Key Key)> _entries = new();

    public void Push(int position, Key key)
    {
        while (_entries.TryPeekBack(out var back) && TOrder.IsDominatedBy(back.Key, key))
        {
            _entries.TryPopBack(out _);
        }

        _entries.PushBack((position, key));
    }

    // Drops every front entry whose position is below firstPosition - the ones the window has slid past.
    public void EvictBefore(int firstPosition)
    {
        while (_entries.TryPeekFront(out var front) && front.Position < firstPosition)
        {
            _entries.TryPopFront(out _);
        }
    }

    public bool TryPeekFront(out (int Position, Key Key) front) => _entries.TryPeekFront(out front);

    public bool TryPopFront(out (int Position, Key Key) front) => _entries.TryPopFront(out front);
}
