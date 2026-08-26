namespace DSAExperimentation.DataStructures.Sequence;

// Satisfies IRandomAccessSequence's O(1) obligation via a raw array's own
// bounds-checked indexer - the simplest possible proof the contract is satisfiable
// at all.
internal readonly struct ArraySequence<Element>(Element[] items) : IRandomAccessSequence<Element>
{
    public int Length => items.Length;

    public Element Get(int index) => items[index];
}
