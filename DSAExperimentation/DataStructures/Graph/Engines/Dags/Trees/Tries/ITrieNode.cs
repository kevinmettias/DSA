namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A trie node as the walk sees it: an object with an identity of its own, holding its children in a
// store kept inline as a field. What else a node carries is its payload, and the payload is the
// node class a configuration injects, because a payload that is not needed must cost no bytes and
// an empty struct field still takes one, which pads a node of two children from 32 bytes to 40:
// - TrieNode<TStore, TValue> carries a value and whether a key ends on it, for a map whose keys may
//   end anywhere, one a prefix of another.
// - BitTrieNode carries nothing: every key of a bit trie is 32 letters long, so a node at that
//   depth is a key's end and no node above it is, and presence is implied by the path.
// The store names its node class, and a node is made only by its store, through New.
/// <summary>A trie node: an object holding its children in an inline store.</summary>
/// <typeparam name="TSelf">The node type itself.</typeparam>
/// <typeparam name="TStore">How the node keeps its children.</typeparam>
public interface ITrieNode<TSelf, TStore>
    where TSelf : class, ITrieNode<TSelf, TStore>
    where TStore : struct
{
    /// <summary>Gets the node's children, by reference, so that adding a child changes this node's own store.</summary>
    ref TStore Children { get; }

    /// <summary>Makes a new node with no children and no payload set.</summary>
    /// <returns>A new node.</returns>
    static abstract TSelf New();
}
