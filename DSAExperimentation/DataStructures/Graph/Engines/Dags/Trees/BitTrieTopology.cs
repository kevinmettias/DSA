using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// Mirrors BinaryTreeTopology exactly: a one-line witness over a node type that
// already carries everything GetChildren needs. ITreeTopology (unique ancestry, no
// sharing) is the correct tier - a bit-trie built from Insert's own Zero/One
// assignments has no way to express a shared or cyclic node, the same argument
// BinaryTreeTopology's own doc comment already makes for Left/Right.
internal readonly struct BitTrieTopology : ITreeTopology<BitTrieNode, BitTrieChildren>
{
    public static BitTrieChildren GetChildren(BitTrieNode node) => new(node);
}
