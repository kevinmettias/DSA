namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The one trie's counted bit trie as an IXorMultiset: a struct view that holds the CountedBitTrie
// and forwards every member, so code generic over the multiset is compiled for the template with
// direct calls, as BitTrieSet makes BitTrie a maximum-XOR set.
/// <summary>A <see cref="CountedBitTrie"/> seen as a XOR multiset.</summary>
/// <param name="trie">The trie that holds the values, read and written in place.</param>
public readonly struct CountedBitTrieMultiset(CountedBitTrie trie)
    : IXorMultiset<CountedBitTrieMultiset>
{
    /// <inheritdoc/>
    public int Count => trie.Count;

    /// <inheritdoc/>
    public static CountedBitTrieMultiset Empty() => new(new CountedBitTrie());

    /// <inheritdoc/>
    public void Insert(int value) => trie.Insert(value);

    /// <inheritdoc/>
    public bool TryRemove(int value) => trie.TryRemove(value);

    /// <inheritdoc/>
    public bool TryMaxXor(int value, out int result) => trie.TryMaxXor(value, out result);

    /// <inheritdoc/>
    public int CountXorBelow(int value, int limit) => trie.CountXorBelow(value, limit);
}
