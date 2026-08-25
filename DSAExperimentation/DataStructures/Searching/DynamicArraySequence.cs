using DSAExperimentation.Algorithms.DynamicArray;

namespace DSAExperimentation.DataStructures.Searching;

// Satisfies IRandomAccessSequence's O(1) obligation for a different reason than
// ArraySequence: DynamicArray<T>.Get is itself array-indexer-backed after one bounds
// check. This required zero changes to DynamicArray.cs, built for Stack's growth
// needs - proving the same algorithm runs over a structure built for an unrelated
// purpose (see ARCHITECTURE.md §9.4 for why this cross-domain composition is sound).
internal readonly struct DynamicArraySequence<T>(DynamicArray<T> items) : IRandomAccessSequence<T>
{
    public int Length => items.Count;

    public T Get(int index) => items.Get(index);
}
