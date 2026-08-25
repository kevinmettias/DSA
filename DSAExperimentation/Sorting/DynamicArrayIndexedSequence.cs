using DSAExperimentation.Collections.DynamicArray;

namespace DSAExperimentation.Sorting;

// Satisfies IIndexedSequence's obligation for a different reason than ArrayIndexedSequence:
// DynamicArray<T>.Get/Set are both already array-indexer-backed after one bounds check, and
// DynamicArray<T> is itself a reference type, so Set's mutation survives every copy of this
// struct. This required zero changes to DynamicArray.cs, built for Stack's growth needs - the
// same one-directional cross-domain composition ARCHITECTURE.md §9.4 already establishes for
// Searching.DynamicArraySequence, replayed here for Sorting.
internal readonly struct DynamicArrayIndexedSequence<T>(DynamicArray<T> items) : IIndexedSequence<T>
{
    public int Length => items.Count;

    public T Get(int index) => items.Get(index);

    public void Set(int index, T value) => items.Set(index, value);
}
