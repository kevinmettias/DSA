namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// How a trie node keeps its children, the one thing about a node's links that tells one trie
// configuration from another: the walk down a key is the same for every store. A node holds its
// store as a struct field, and all it asks of the store's type is an empty store to start with.
// Reaching a child by the letter on its edge is ILetteredTrieChildStore's, and reaching one by its
// rank IOrderedTrieChildStore's, so a node, and a tree engine walking nodes, need neither a letter
// type nor a key. The store names the node class its children are, and the node class is where a
// configuration's payload lives (ITrieNode).
/// <summary>How a trie node keeps its children: a store a new node starts empty.</summary>
/// <typeparam name="TSelf">The store type itself.</typeparam>
/// <typeparam name="TNode">The node class the store's children are.</typeparam>
public interface ITrieChildStore<TSelf, TNode>
    where TSelf : struct, ITrieChildStore<TSelf, TNode>
    where TNode : class
{
    /// <summary>Makes the store a new node starts with, holding no children.</summary>
    /// <returns>An empty store.</returns>
    static abstract TSelf Empty();
}
