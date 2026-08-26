using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.DataStructures.Sequence;

// Satisfies IRandomAccessSequence's O(1) obligation for a different reason than
// ArraySequence: DynamicArray<Element>.Get is itself array-indexer-backed after one bounds
// check. This required zero changes to DynamicArray.cs, built for Stack's growth
// needs - proving the same algorithm runs over a structure built for an unrelated
// purpose (see ARCHITECTURE.md §9.4 for why this cross-domain composition is sound).
internal readonly struct DynamicArraySequence<Element>(DynamicArray<Element> items) : IRandomAccessSequence<Element>
{
    public int Length => items.Count;

    public Element Get(int index) => items.Get(index);
}
