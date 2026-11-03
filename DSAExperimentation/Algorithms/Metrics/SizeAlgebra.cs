using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.Algorithms.Metrics;

/// <summary>A fold algebra for a tree's size: its number of nodes, 0 for no tree.</summary>
public readonly struct SizeAlgebra<TNode> : IFoldAlgebra<TNode, int>
{
    public int Empty => 0;

    public int Combine(TNode node, ReadOnlySpan<int> children)
    {
        var size = 1;

        foreach (var childSize in children)
        {
            size += childSize;
        }

        return size;
    }
}
