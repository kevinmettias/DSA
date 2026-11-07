namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The pooled counted bit trie as an IXorMultiset: a struct view that holds the PooledCountedBitTrie
// and forwards every member, as CountedBitTrieMultiset does for the template. The trie stays a
// class that its users call directly, as they did before the contract existed, so the view adds
// nothing to their code; it is what the contract suite and the strategy benchmark run.
/// <summary>A <see cref="PooledCountedBitTrie"/> seen as a XOR multiset.</summary>
/// <param name="trie">The trie that holds the values, read and written in place.</param>
public readonly struct PooledCountedBitTrieMultiset(PooledCountedBitTrie trie)
    : IXorMultiset<PooledCountedBitTrieMultiset>
{
    /// <inheritdoc/>
    public int Count => trie.Count;

    /// <inheritdoc/>
    public static PooledCountedBitTrieMultiset Empty() => new(new PooledCountedBitTrie());

    /// <inheritdoc/>
    public void Insert(int value) => trie.Insert(value);

    /// <inheritdoc/>
    public bool TryRemove(int value) => trie.TryRemove(value);

    /// <inheritdoc/>
    public bool TryMaxXor(int value, out int result) => trie.TryMaxXor(value, out result);

    /// <inheritdoc/>
    public int CountXorBelow(int value, int limit) => trie.CountXorBelow(value, limit);
}
