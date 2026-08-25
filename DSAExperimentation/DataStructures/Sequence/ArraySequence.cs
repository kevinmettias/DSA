namespace DSAExperimentation.DataStructures.Sequence;

// Satisfies IRandomAccessSequence's O(1) obligation via a raw array's own
// bounds-checked indexer - the simplest possible proof the contract is satisfiable
// at all.
internal readonly struct ArraySequence<T>(T[] items) : IRandomAccessSequence<T>
{
    public int Length => items.Length;

    public T Get(int index) => items[index];
}
