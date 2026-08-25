using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Contracts.Ordering;

internal readonly struct TrieTopology : ITreeTopology<TrieNode, SparseArrayChildren<TrieNode>>
{
    public static SparseArrayChildren<TrieNode> GetChildren(TrieNode node) => new(node.Children);
}
