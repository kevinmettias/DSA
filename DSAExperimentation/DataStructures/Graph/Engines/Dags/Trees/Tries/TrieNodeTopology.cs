namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The tree-topology witness of a trie of any node class whose store is ordered, BitTrie's among
// them: what TrieTopology is to a map's TrieNode, for every payload (TrieNodeChildren says why both
// exist).
/// <summary>The tree-topology witness of a trie of any node class whose child store is ordered.</summary>
/// <typeparam name="TNode">The node class.</typeparam>
/// <typeparam name="TStore">The node's ordered child store.</typeparam>
public readonly struct TrieNodeTopology<TNode, TStore> : ITreeTopology<TNode, TrieNodeChildren<TNode, TStore>>
    where TNode : class, ITrieNode<TNode, TStore>
    where TStore : struct, IOrderedTrieChildStore<TStore, TNode>
{
    /// <inheritdoc/>
    public static TrieNodeChildren<TNode, TStore> GetChildren(TNode node) => new(node);
}
