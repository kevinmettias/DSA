using DSAExperimentation.DataStructures.Alphabets;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A node's children in letter order, read through the node's mask: the count is a popcount and the
// child of a rank is found by the layout, so a generic tree engine sweeping Get(0) to Get(Count - 1)
// never scans an empty slot, under either layout. A foreach over it gives each child with its letter,
// for a walk that needs to know which letter it followed.
/// <summary>An alphabet-trie node's children, in letter order.</summary>
/// <typeparam name="TAlphabet">The letters a key is written in.</typeparam>
/// <typeparam name="TLayout">Where the node stores each child.</typeparam>
/// <typeparam name="TValue">The value a key maps to.</typeparam>
/// <param name="node">The node whose children to present.</param>
public readonly struct AlphabetTrieChildren<TAlphabet, TLayout, TValue>(AlphabetTrieNode<TAlphabet, TLayout, TValue> node)
    : IChildren<AlphabetTrieNode<TAlphabet, TLayout, TValue>>
    where TAlphabet : struct, IAlphabet
    where TLayout : struct, ITrieLayout
{
    /// <inheritdoc/>
    public int Count => node.ChildCount;

    /// <inheritdoc/>
    public AlphabetTrieNode<TAlphabet, TLayout, TValue> Get(int index) => node.ChildOfRank(index);

    /// <summary>Enumerates the children in letter order, each with its letter.</summary>
    /// <returns>An enumerator over the node's children.</returns>
    public AlphabetTrieChildEnumerator<TAlphabet, TLayout, TValue> GetEnumerator() => new(node);
}
