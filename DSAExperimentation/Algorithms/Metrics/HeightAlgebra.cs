using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.Algorithms.Metrics;

/// <summary>A fold algebra for a tree's height in nodes: 1 for a leaf, 0 for no tree.</summary>
public readonly struct HeightAlgebra<TNode> : IFoldAlgebra<TNode, int>
{
    public int Empty => 0;

    public int Combine(TNode node, IReadOnlyList<int> children)
        => 1 + (children.Count == 0 ? 0 : children.Max());
}
