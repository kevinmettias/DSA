using System.Diagnostics.CodeAnalysis;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A node of a CountedBitTrie: its two children and a count, the payload of a bit trie over a
// multiset whose values can be removed. The count is how many held values, duplicates included,
// have a path through the node, so a node's count is the sum of its children's, and a removal
// lowers the counts along a path but leaves its nodes: a child holds a value only while its count
// is positive, whether it exists or not. The node is the object header, its two child references
// and the count, 40 bytes with the padding an object rounds up to, where BitTrieNode's is 32.
/// <summary>
/// A <see cref="CountedBitTrie"/> node: its children, one per binary digit, and how many held
/// values pass through it.
/// </summary>
public sealed class CountedBitTrieNode : IBitTrieNode<CountedBitTrieNode>
{
    private BinarySlots<CountedBitTrieNode> _children;

    /// <inheritdoc/>
    public ref BinarySlots<CountedBitTrieNode> Children => ref _children;

    /// <summary>
    /// Gets how many held values, duplicates included, have a path through this node.
    /// </summary>
    public int Count { get; internal set; }

    internal CountedBitTrieNode()
    {
    }

    /// <inheritdoc/>
    static CountedBitTrieNode ITrieNode<CountedBitTrieNode, BinarySlots<CountedBitTrieNode>>.New()
        => new();

    /// <inheritdoc/>
    static bool IBitTrieNode<CountedBitTrieNode>.IsLive(
        [NotNullWhen(true)] CountedBitTrieNode? child) =>
        child is not null && child.Count > 0;
}
