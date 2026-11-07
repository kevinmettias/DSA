namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A node of a BitTrie: its two children and nothing else, the payload of a trie whose keys are all
// 32 letters long. A node at depth 32 is a key's end and no node above it is, so whether a key is
// held is implied by its path, and a mark or a value on every node would be bytes no walk reads.
// The node is the object header and its two child references, 32 bytes, the hand-written bit
// trie's.
/// <summary>A <see cref="BitTrie"/> node: its children, one per binary digit, and no payload.</summary>
public sealed class BitTrieNode : ITrieNode<BitTrieNode, BinarySlots<BitTrieNode>>
{
    private BinarySlots<BitTrieNode> _children;

    /// <inheritdoc/>
    public ref BinarySlots<BitTrieNode> Children => ref _children;

    internal BitTrieNode()
    {
    }

    /// <inheritdoc/>
    static BitTrieNode ITrieNode<BitTrieNode, BinarySlots<BitTrieNode>>.New() => new();
}
