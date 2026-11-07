using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// Any trie node's children as a generic tree engine sweeps them, Get(0) to Get(Count - 1), in its
// ordered store's order: the sweep TrieChildren gives a map's TrieNode, for a node class of any
// payload, BitTrieNode's among them. TrieChildren stays for TrieNode because it is generic over
// value types only, so a word trie's engines are compiled for its exact node; this one is generic
// over the node class, a reference type, so engines walking it share code across node classes.
/// <summary>A trie node's children in its store's order, for a tree engine to sweep.</summary>
/// <typeparam name="TNode">The node class.</typeparam>
/// <typeparam name="TStore">The node's ordered child store.</typeparam>
/// <param name="node">The node whose children to list.</param>
public readonly struct TrieNodeChildren<TNode, TStore>(TNode node) : IChildren<TNode>
    where TNode : class, ITrieNode<TNode, TStore>
    where TStore : struct, IOrderedTrieChildStore<TStore, TNode>
{
    /// <inheritdoc/>
    public int Count => node.Children.Count;

    /// <inheritdoc/>
    public TNode Get(int index) => node.Children.ChildOfRank(index);
}
