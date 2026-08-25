namespace DSAExperimentation.DataStructures.Sequence;

// Satisfies IIndexedSequence's doubled O(1) obligation via a raw array's own bounds-checked
// indexer for both directions - the simplest possible proof the contract is satisfiable at all -
// backed by a reference type, so Set's mutation is visible across every copy of this struct (see
// IIndexedSequence.cs).
internal readonly struct ArrayIndexedSequence<T>(T[] items) : IIndexedSequence<T>
{
    public int Length => items.Length;

    public T Get(int index) => items[index];

    public void Set(int index, T value) => items[index] = value;
}
