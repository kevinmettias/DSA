namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A store whose children are reached by the letter labelling the edge to each, the store the trie's
// key walk reads. Which letters a store takes is its own invariant: an alphabet's characters for
// AlphabetSlots, any character for HashedChildren, a binary digit for BinarySlots. The letter type
// ties a store to the keys that can be walked over it, so a trie whose key reads bits cannot be
// given a store keyed by characters: the compiler refuses the pair. A letter of the right type that
// the store does not take is the caller's precondition to keep, as the alphabet's own IndexOf
// states.
/// <summary>How a trie node keeps its children, reached by the letter on the edge to each.</summary>
/// <typeparam name="TSelf">The store type itself.</typeparam>
/// <typeparam name="TNode">The node class the store's children are.</typeparam>
/// <typeparam name="TLetter">The letter labelling each edge.</typeparam>
public interface ILetteredTrieChildStore<TSelf, TNode, TLetter> : ITrieChildStore<TSelf, TNode>
    where TSelf : struct, ILetteredTrieChildStore<TSelf, TNode, TLetter>
    where TNode : class
{
    /// <summary>Finds the child on the edge labelled by a letter.</summary>
    /// <param name="key">The letter on the edge.</param>
    /// <returns>The child, or <c>null</c> when the node has none on that edge.</returns>
    TNode? ChildFor(TLetter key);

    /// <summary>Finds the child on the edge labelled by a letter, adding a new one when there is none.</summary>
    /// <param name="key">The letter on the edge.</param>
    /// <returns>The child on that edge.</returns>
    TNode GetOrAddChildFor(TLetter key);
}
