using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.Algorithms.Metrics;

/// <summary>A fold algebra for a tree's height in nodes: 1 for a leaf, 0 for no tree.</summary>
public readonly struct HeightAlgebra<TNode> : IFoldAlgebra<TNode, int>
{
    public int Empty => 0;

    public int Combine(TNode node, ReadOnlySpan<int> children)
    {
        var tallestChild = 0;

        foreach (var childHeight in children)
        {
            tallestChild = Math.Max(tallestChild, childHeight);
        }

        return 1 + tallestChild;
    }
}
