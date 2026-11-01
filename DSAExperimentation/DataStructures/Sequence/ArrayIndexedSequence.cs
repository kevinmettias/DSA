namespace DSAExperimentation.DataStructures.Sequence;

// Satisfies IIndexedSequence's doubled O(1) obligation via a raw array's own bounds-checked
// indexer for both directions - the simplest possible proof the contract is satisfiable at all -
// backed by a reference type, so Set's mutation is visible across every copy of this struct (see
// IIndexedSequence.cs).
/// <summary>An <see cref="IIndexedSequence{Element}"/> backed by a caller's array.</summary>
/// <param name="items">The array read and written in place, shared rather than copied.</param>
public readonly struct ArrayIndexedSequence<Element>(Element[] items) : IIndexedSequence<Element>
{
    public int Length => items.Length;

    public Element Get(int index) => items[index];

    public void Set(int index, Element value) => items[index] = value;
}
