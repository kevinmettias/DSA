namespace DSAExperimentation.DataStructures.Sequence;

// Satisfies IIndexedSequence's doubled O(1) obligation via a raw array's own bounds-checked
// indexer for both directions - the simplest possible proof the contract is satisfiable at all -
// backed by a reference type, so Set's mutation is visible across every copy of this struct (see
// IIndexedSequence.cs).
internal readonly struct ArrayIndexedSequence<Element>(Element[] items) : IIndexedSequence<Element>
{
    public int Length => items.Length;

    public Element Get(int index) => items[index];

    public void Set(int index, Element value) => items[index] = value;
}
