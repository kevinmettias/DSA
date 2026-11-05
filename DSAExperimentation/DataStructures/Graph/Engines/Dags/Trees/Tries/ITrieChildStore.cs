namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// How a trie node keeps its children, the one thing that tells one trie configuration from another:
// everything else about a trie, the walk down a key and the values at its nodes, is Trie<TStore,
// TValue>'s and the same for every store. A node holds its store as a struct field and reaches a child
// by the character that labels the edge to it. Which characters a store takes is its own invariant:
// an alphabet's letters for AlphabetSlots, any character for HashedChildren. A character outside a
// store's set is the caller's precondition to keep, as the alphabet's own IndexOf states.
/// <summary>How a trie node keeps its children, reached by the character on the edge to each.</summary>
/// <typeparam name="TSelf">The store type itself.</typeparam>
/// <typeparam name="TValue">The value the trie stores under each key.</typeparam>
public interface ITrieChildStore<TSelf, TValue>
    where TSelf : struct, ITrieChildStore<TSelf, TValue>
{
    /// <summary>Makes the store a new node starts with, holding no children.</summary>
    /// <returns>An empty store.</returns>
    static abstract TSelf Empty();

    /// <summary>Finds the child on the edge labelled by a character.</summary>
    /// <param name="key">The character on the edge.</param>
    /// <returns>The child, or <c>null</c> when the node has none on that edge.</returns>
    TrieNode<TSelf, TValue>? ChildFor(char key);

    /// <summary>Finds the child on the edge labelled by a character, adding a new one when there is none.</summary>
    /// <param name="key">The character on the edge.</param>
    /// <returns>The child on that edge.</returns>
    TrieNode<TSelf, TValue> GetOrAddChildFor(char key);
}
