namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The one trie as a set: a map to bool whose Add stores true, every other member the map's own, so the
// view holds one reference and walks nothing itself. A key is present when its node holds a value, so
// the bool stored there is never read.
/// <summary>A <see cref="Trie{TStore, TValue}"/> seen as a set of its keys.</summary>
/// <typeparam name="TStore">How each node keeps its children.</typeparam>
/// <param name="keys">The trie that holds the keys, read and written in place.</param>
public readonly struct PrefixSet<TStore>(Trie<TStore, bool> keys) : IPrefixSet
    where TStore : struct, ILetteredTrieChildStore<TStore, TrieNode<TStore, bool>, char>
{
    /// <inheritdoc/>
    public int Count => keys.Count;

    /// <inheritdoc/>
    public void Add(string key) => keys.Set(key, true);

    /// <inheritdoc/>
    public bool HasKey(string key) => keys.HasKey(key);

    /// <inheritdoc/>
    public bool HasPrefix(string prefix) => keys.HasPrefix(prefix);
}
