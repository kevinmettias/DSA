using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.Algorithms.Metrics;

internal readonly struct HeightAlgebra<TNode> : IFoldAlgebra<TNode, int>
{
    public int Empty => 0;

    public int Combine(TNode node, IReadOnlyList<int> children)
        => 1 + (children.Count == 0 ? 0 : children.Max());
}
