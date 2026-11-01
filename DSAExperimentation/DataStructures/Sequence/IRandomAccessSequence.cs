namespace DSAExperimentation.DataStructures.Sequence;

// Parallels IChildren<TNode>: Get is a named method, not an indexer, so
// implementations stay thin structs the JIT specializes to direct, non-virtual,
// non-boxing calls. Length, not Count, is deliberate - this models "how many things
// to index into" the way arrays/strings do, not "how many children" the way
// IChildren does.
//
// Get is assumed O(1) - that's what lets BinarySearch's O(log n) claim hold. This is
// new relative to IChildren, whose own doc comment never states a cost obligation,
// because nothing in Graph/** depends on Get's cost (see ARCHITECTURE.md §2). Here,
// something does, so the obligation belongs on the contract itself.
/// <summary>A read-only sequence whose elements can be read by position in constant time.</summary>
public interface IRandomAccessSequence<Element>
{
    /// <summary>The number of elements, so valid positions run from 0 through <c>Length - 1</c>.</summary>
    int Length { get; }

    /// <summary>Reads the element at a position, in constant time.</summary>
    /// <param name="index">The position to read, from 0 through <c>Length - 1</c>.</param>
    /// <returns>The element at that position.</returns>
    Element Get(int index);
}
