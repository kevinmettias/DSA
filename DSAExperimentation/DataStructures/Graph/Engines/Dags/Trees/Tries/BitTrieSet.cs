namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The one trie's bit trie as an IMaxXorSet: a struct view that holds the BitTrie and forwards every
// member, so code generic over the set, LC 2935's bucket strategy, is compiled for the template
// with direct calls, as PrefixSet makes the word trie a set.
/// <summary>A <see cref="BitTrie"/> seen as a maximum-XOR set.</summary>
/// <param name="trie">The trie that holds the values, read and written in place.</param>
public readonly struct BitTrieSet(BitTrie trie) : IMaxXorSet<BitTrieSet>
{
    /// <inheritdoc/>
    public int Count => trie.Count;

    /// <inheritdoc/>
    public static BitTrieSet Empty() => new(new BitTrie());

    /// <inheritdoc/>
    public void Insert(int value) => trie.Insert(value);

    /// <inheritdoc/>
    public bool HasKey(int value) => trie.HasKey(value);

    /// <inheritdoc/>
    public bool HasPrefix(int value, int bits) => trie.HasPrefix(value, bits);

    /// <inheritdoc/>
    public bool TryMaxXor(int value, out int result) => trie.TryMaxXor(value, out result);
}
