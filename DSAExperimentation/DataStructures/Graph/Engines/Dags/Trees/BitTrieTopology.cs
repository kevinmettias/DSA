using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// Mirrors BinaryTreeTopology exactly: a one-line witness over a node type that
// already carries everything GetChildren needs. ITreeTopology (unique ancestry, no
// sharing) is the correct tier - a bit-trie built from Insert's own Zero/One
// assignments has no way to express a shared or cyclic node, the same argument
// BinaryTreeTopology's own doc comment already makes for Left/Right.
/// <summary>The tree topology over <see cref="BitTrieNode"/>, giving a node's non-null Zero and One.</summary>
public readonly struct BitTrieTopology : ITreeTopology<BitTrieNode, BitTrieChildren>
{
    /// <summary>Returns a node's present children, the 0 branch before the 1 branch.</summary>
    /// <param name="node">The node whose children to return.</param>
    /// <returns>A view of the node's non-null Zero and One, in that order.</returns>
    public static BitTrieChildren GetChildren(BitTrieNode node) => new(node);
}
