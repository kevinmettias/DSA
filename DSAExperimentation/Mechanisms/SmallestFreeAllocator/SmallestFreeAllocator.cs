using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Mechanisms.SmallestFreeAllocator;

// Hands out whole numbers from a first one upward, always the smallest not currently held, and takes
// back the numbers it handed out. A mechanism under ARCHITECTURE.md §17.6b: Allocate returns a number
// the caller never stored. The numbers from the next never-issued one upward are all free and need no
// storage, so a counter stands for them; only a number released below the counter needs a place, in
// a min-heap. Every released number is below the counter, so the heap's root, when there is one, is
// the smallest free number outright, and the two sources never need comparing.
//
// A flag per number, by its offset from the first, marks the released ones, so that a number not held
// - released already, or never handed out - is turned away and no number is ever handed out twice at
// once. The numbers handed out are dense, so the flags are an array, doubled as far as the largest
// number released: a hash set of the released numbers cost LC 1845's seat manager 63-77% of its time
// and 5-11 times its allocation, and a DynamicArray grown one flag at a time 25-39%.
/// <summary>Hands out the smallest whole number not currently held, from a first number upward.</summary>
/// <param name="first">The smallest number it hands out.</param>
public sealed class SmallestFreeAllocator(int first)
{
    private readonly Heap<int, MinHeapOrder<int>> _releasedInOrder = new();
    private readonly int _first = first;
    private bool[] _isReleasedByOffset = [];
    private int _nextFresh = first;

    /// <summary>Hands out the smallest number not currently held, which is then held.</summary>
    /// <returns>The number handed out.</returns>
    public int Allocate()
    {
        if (_releasedInOrder.TryPop(out var released))
        {
            _isReleasedByOffset[released - _first] = false;
            return released;
        }

        return _nextFresh++;
    }

    /// <summary>Takes back a held number, so that a later <see cref="Allocate"/> can hand it out again.</summary>
    /// <param name="number">The number to take back.</param>
    /// <returns><see langword="true"/> when the number was held; <see langword="false"/> when it was released already or never handed out.</returns>
    public bool TryRelease(int number)
    {
        if (!IsIssued(number) || IsReleased(number))
        {
            return false;
        }

        MarkReleased(number);
        _releasedInOrder.Push(number);
        return true;
    }

    // Handed out at some point: at or above the first number and below the next never-issued one.
    private bool IsIssued(int number) => number >= _first && number < _nextFresh;

    // A number past the flags' end has never been released.
    private bool IsReleased(int number)
    {
        var offset = number - _first;

        return offset < _isReleasedByOffset.Length && _isReleasedByOffset[offset];
    }

    private void MarkReleased(int number)
    {
        var offset = number - _first;

        if (offset >= _isReleasedByOffset.Length)
        {
            var grown = Math.Max(_isReleasedByOffset.Length * ArrayGrowth.GrowthFactor, offset + 1);
            Array.Resize(ref _isReleasedByOffset, grown);
        }

        _isReleasedByOffset[offset] = true;
    }
}
