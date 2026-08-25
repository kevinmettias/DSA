namespace DSAExperimentation.DataStructures.Sorting;

// Sorting is a distinct domain from Searching, so per ARCHITECTURE.md §5.5 it defines its own
// Representation contract rather than reusing Searching.IRandomAccessSequence<T> - domain
// ownership is the reason, independent of whether the two domains' operation sets happen to
// overlap. That separation is doubly warranted here: IRandomAccessSequence<T> is read-only and
// could not satisfy Set even if reuse were otherwise permitted.
//
// Get and Set are both assumed O(1) - MergeSort's O(n log n) claim depends on both, doubled
// relative to IRandomAccessSequence<T>'s single-Get obligation.
//
// Any implementation must back Get/Set with a reference type, not a value type: a TSequence
// witness is a struct passed by value into every recursive call MergeSort makes, so Set's
// mutation is only visible across those copies if the copied struct still aliases the same
// backing store. A witness wrapping a value-type field directly would compile fine and silently
// drop every Set made through a copy - a correctness law this repo's Get-only contracts never
// had to state.
internal interface IIndexedSequence<T>
{
    int Length { get; }

    T Get(int index);

    void Set(int index, T value);
}
