namespace DSAExperimentation.DataStructures.Sequence;

// Satisfies IRandomAccessSequence's O(1) obligation via a raw array's own
// bounds-checked indexer - the simplest possible proof the contract is satisfiable
// at all.
/// <summary>Presents an array as a read-only random-access sequence, without copying it.</summary>
/// <param name="items">The array whose elements, in order, make up the sequence.</param>
public readonly struct ArraySequence<Element>(Element[] items) : IRandomAccessSequence<Element>
{
    public int Length => items.Length;

    public Element Get(int index) => items[index];
}
