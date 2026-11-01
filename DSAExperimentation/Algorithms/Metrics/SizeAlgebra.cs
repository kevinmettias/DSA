using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.Algorithms.Metrics;

/// <summary>A fold algebra for a tree's size: its number of nodes, 0 for no tree.</summary>
public readonly struct SizeAlgebra<TNode> : IFoldAlgebra<TNode, int>
{
    public int Empty => 0;

    public int Combine(TNode node, IReadOnlyList<int> children)
        => 1 + children.Sum();
}
