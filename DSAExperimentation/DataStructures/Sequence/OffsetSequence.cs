namespace DSAExperimentation.DataStructures.Sequence;

// A window into an existing array: Get(index) = items[start + index] - the same
// O(1) proof ArraySequence gives for a whole array, restricted to one contiguous
// run of it. Lets a caller hand BinarySearch a suffix (or any subrange) without
// copying it into a fresh array first.
internal readonly struct OffsetSequence<Element>(Element[] items, int start, int length)
    : IRandomAccessSequence<Element>
{
    public int Length => length;

    public Element Get(int index) => items[start + index];
}
