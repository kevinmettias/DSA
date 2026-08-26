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
internal interface IIndexedSequence<Element>
{
    int Length { get; }

    Element Get(int index);

    void Set(int index, Element value);
}
