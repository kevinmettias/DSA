using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A trie node's children as a generic tree engine sweeps them, Get(0) to Get(Count - 1), in the
// store's order. Only an ordered store has an order to sweep, so the constraint keeps a hashed trie's
// node out.
/// <summary>A trie node's children in its store's order, for a tree engine to sweep.</summary>
/// <typeparam name="TStore">The trie's ordered child store.</typeparam>
/// <typeparam name="TValue">The value the trie stores under each key.</typeparam>
/// <param name="node">The node whose children to list.</param>
public readonly struct TrieChildren<TStore, TValue>(TrieNode<TStore, TValue> node) : IChildren<TrieNode<TStore, TValue>>
    where TStore : struct, IOrderedTrieChildStore<TStore, TValue>
{
    /// <inheritdoc/>
    public int Count => node.Children.Count;

    /// <inheritdoc/>
    public TrieNode<TStore, TValue> Get(int index) => node.Children.ChildOfRank(index);
}
