namespace DSAExperimentation.DataStructures.Sequence;

// A window into an existing array: Get(index) = items[start + index] - the same
// O(1) proof ArraySequence gives for a whole array, restricted to one contiguous
// run of it. Lets a caller hand BinarySearch a suffix (or any subrange) without
// copying it into a fresh array first.
/// <summary>
/// Presents a contiguous run of an array as a read-only random-access sequence, without copying it.
/// </summary>
/// <param name="items">The array the run lies in.</param>
/// <param name="start">The index in <paramref name="items"/> of the run's first element.</param>
/// <param name="length">How many elements the run holds; it must not extend past the array's end.</param>
public readonly struct OffsetSequence<Element>(Element[] items, int start, int length)
    : IRandomAccessSequence<Element>
{
    public int Length => length;

    public Element Get(int index) => items[start + index];
}
