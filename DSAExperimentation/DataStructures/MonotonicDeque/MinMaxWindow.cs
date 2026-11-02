namespace DSAExperimentation.DataStructures.MonotonicDeque;

// A sliding window's minimum and maximum together, for the windows bounded by their range rather than
// by their width: grow at the right, and while max - min, or a cost built on it, is too large, step the
// left edge forward. Six solutions kept this pair by hand as two MonotonicDeques, one under each order,
// pushing every value into both and evicting from both at every step of the left edge.
//
// It adds no axis of its own. Both orders are fixed, because a range is the two extrema together: a
// caller that wants one extremum wants a MonotonicDeque. The range is reported as keys alone, since
// those six compared the two fronts' keys and never read their positions; a caller that needs where an
// extremum sits keeps a MonotonicDeque of its own.
//
// Precondition law, unchecked, as MonotonicDeque's: positions are pushed in non-decreasing order. Every
// push and every eviction reaches both deques, so both hold the newest position still in the window and
// are empty together. Complexity law (§8): each entry enters and leaves each deque at most once, so a
// whole sweep is O(n) amortized.
//
// It is a readonly struct, not a class, so that it is no object of its own. As a class it put one more
// pointer between its caller and each deque, a load the JIT repeated on every push, eviction and peek:
// LC 1438's sweep went from 23 pointer loads to 32 and ran 8-18% slower, LC 2762's 16-42%. Hold it in
// a local and pass it by value. A struct kept in a class's field is reached by address, and the JIT
// takes memory reached that way to be overwritten by any store, so a loop that stores anything reloads
// both deques on every pass; LC 2762's shrink loop did, where a local's two references are loaded once
// (ARCHITECTURE 19.1). Create one with new(): a default MinMaxWindow holds no deques.
/// <summary>A sliding window's minimum and maximum together, each in amortized O(1) per step.</summary>
public readonly struct MinMaxWindow<Key>
    where Key : IComparable<Key>
{
    private readonly MonotonicDeque<Key, MinWindowOrder<Key>> _minimums;
    private readonly MonotonicDeque<Key, MaxWindowOrder<Key>> _maximums;

    /// <summary>Creates an empty window.</summary>
    public MinMaxWindow()
    {
        _minimums = new();
        _maximums = new();
    }

    /// <summary>Adds a key at a position, evicting every entry it rules out as the minimum or the maximum.</summary>
    /// <param name="position">The key's position, which must not be below any position already pushed.</param>
    /// <param name="key">The key.</param>
    public void Push(int position, Key key)
    {
        _minimums.Push(position, key);
        _maximums.Push(position, key);
    }

    /// <summary>Drops every entry whose position is below <paramref name="firstPosition"/>.</summary>
    /// <param name="firstPosition">The first position still inside the window.</param>
    public void EvictBefore(int firstPosition)
    {
        _minimums.EvictBefore(firstPosition);
        _maximums.EvictBefore(firstPosition);
    }

    // Both fronts are read with `&` rather than `&&`, so both are assigned on every path; the deques are
    // empty together, so the two answers never differ.
    /// <summary>Reads the smallest and largest keys still in the window.</summary>
    /// <param name="minimum">The smallest key, or <c>default</c> when the window is empty.</param>
    /// <param name="maximum">The largest key, or <c>default</c> when the window is empty.</param>
    /// <returns>true when the window holds a key; false when it is empty.</returns>
    public bool TryPeekRange(out Key minimum, out Key maximum)
    {
        var isHeld = _minimums.TryPeekFront(out var lowest) & _maximums.TryPeekFront(out var highest);
        minimum = lowest.Key;
        maximum = highest.Key;

        return isHeld;
    }
}
