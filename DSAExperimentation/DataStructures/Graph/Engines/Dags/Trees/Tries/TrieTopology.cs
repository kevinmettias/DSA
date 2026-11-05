namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A trie is a tree whose edges are characters: this witness lets every generic tree engine (TreeMetrics,
// LowestCommonAncestor, TreeFold, Reduce, ...) run on a trie with no trie code of its own, for any store
// whose children are ordered. A hashed store's are not, so it has no witness.
/// <summary>The tree-topology witness of a trie whose child store is ordered.</summary>
/// <typeparam name="TStore">The trie's ordered child store.</typeparam>
/// <typeparam name="TValue">The value the trie stores under each key.</typeparam>
public readonly struct TrieTopology<TStore, TValue> : ITreeTopology<TrieNode<TStore, TValue>, TrieChildren<TStore, TValue>>
    where TStore : struct, IOrderedTrieChildStore<TStore, TValue>
{
    /// <inheritdoc/>
    public static TrieChildren<TStore, TValue> GetChildren(TrieNode<TStore, TValue> node) => new(node);
}
