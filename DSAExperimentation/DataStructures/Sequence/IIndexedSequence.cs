namespace DSAExperimentation.DataStructures.Sequence;

// Sorting is a distinct domain from Searching, so per ARCHITECTURE.md §5's domain-reuse rule it defines its own
// Representation contract rather than reusing Searching.IRandomAccessSequence<Element> - domain
// ownership is the reason, independent of whether the two domains' operation sets happen to
// overlap. That separation is doubly warranted here: IRandomAccessSequence<Element> is read-only and
// could not satisfy Set even if reuse were otherwise permitted.
//
// Get and Set are both assumed O(1) - MergeSort's O(n log n) claim depends on both, doubled
// relative to IRandomAccessSequence<Element>'s single-Get obligation.
//
// Any implementation must back Get/Set with a reference type, not a value type: a TSequence
// witness is a struct passed by value into every recursive call MergeSort makes, so Set's
// mutation is only visible across those copies if the copied struct still aliases the same
// backing store. A witness wrapping a value-type field directly would compile fine and silently
// drop every Set made through a copy - a correctness law this repo's Get-only contracts never
// had to state.
/// <summary>
/// A sequence an algorithm can read and overwrite in place by position, each access in constant time.
/// </summary>
public interface IIndexedSequence<Element>
{
    /// <summary>The number of elements, so valid positions run from 0 through <c>Length - 1</c>.</summary>
    int Length { get; }

    /// <summary>Reads the element at a position, in constant time.</summary>
    /// <param name="index">The position to read, from 0 through <c>Length - 1</c>.</param>
    /// <returns>The element at that position.</returns>
    Element Get(int index);

    /// <summary>
    /// Overwrites the element at a position in constant time, visibly to every copy of this sequence.
    /// </summary>
    /// <param name="index">The position to overwrite, from 0 through <c>Length - 1</c>.</param>
    /// <param name="value">The element to store there.</param>
    void Set(int index, Element value);
}
