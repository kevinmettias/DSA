using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.Algorithms.Metrics;

public readonly struct SizeAlgebra<TNode> : IFoldAlgebra<TNode, int>
{
    public int Empty => 0;

    public int Combine(TNode node, IReadOnlyList<int> children)
        => 1 + children.Sum();
}
