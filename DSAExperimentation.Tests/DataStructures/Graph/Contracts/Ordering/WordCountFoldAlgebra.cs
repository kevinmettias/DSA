using DSAExperimentation.Algorithms.Graph.Engines.Folding;

namespace DSAExperimentation.Tests.DataStructures.Graph.Contracts.Ordering;

internal readonly struct WordCountFoldAlgebra : IFoldAlgebra<TrieNode, int>
{
    public static int Empty => 0;

    public static int Combine(TrieNode node, IReadOnlyList<int> children)
        => (node.IsWord ? 1 : 0) + children.Sum();
}
