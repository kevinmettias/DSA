using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A trie is a tree whose edges are letters: this witness is what lets every generic tree engine
// (TreeMetrics, LowestCommonAncestor, TreeFold, Reduce, ...) run on an alphabet trie with no trie code
// of its own, under any alphabet and either layout.
/// <summary>The tree topology witness for <see cref="AlphabetTrieNode{TAlphabet, TLayout, TValue}"/>.</summary>
/// <typeparam name="TAlphabet">The letters a key is written in.</typeparam>
/// <typeparam name="TLayout">Where each node stores its children.</typeparam>
/// <typeparam name="TValue">The value a key maps to.</typeparam>
public readonly struct AlphabetTrieTopology<TAlphabet, TLayout, TValue>
    : ITreeTopology<AlphabetTrieNode<TAlphabet, TLayout, TValue>, AlphabetTrieChildren<TAlphabet, TLayout, TValue>>
    where TAlphabet : struct, IAlphabet
    where TLayout : struct, ITrieLayout
{
    /// <summary>Lists a trie node's children, in letter order.</summary>
    /// <param name="node">The trie node whose children to list.</param>
    /// <returns>A view over the node's children.</returns>
    public static AlphabetTrieChildren<TAlphabet, TLayout, TValue> GetChildren(AlphabetTrieNode<TAlphabet, TLayout, TValue> node) => new(node);
}
