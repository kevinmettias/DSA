using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.DataStructures.Sequence;

// Satisfies IIndexedSequence's obligation for a different reason than ArrayIndexedSequence:
// DynamicArray<Element>.Get/Set are both already array-indexer-backed after one bounds check, and
// DynamicArray<Element> is itself a reference type, so Set's mutation survives every copy of this
// struct. This required zero changes to DynamicArray.cs, built for Stack's growth needs - the
// same one-directional cross-domain composition ARCHITECTURE.md §9.4 already establishes for
// Searching.DynamicArraySequence, replayed here for Sorting.
internal readonly struct DynamicArrayIndexedSequence<Element>(DynamicArray<Element> items) : IIndexedSequence<Element>
{
    public int Length => items.Count;

    public Element Get(int index) => items.Get(index);

    public void Set(int index, Element value) => items.Set(index, value);
}
