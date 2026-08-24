namespace DSAExperimentation.Graph;

public readonly struct SizeAlgebra<TNode> : IFoldAlgebra<TNode, int>
{
    public static int Empty => 0;

    public static int Combine(TNode node, IReadOnlyList<int> children)
        => 1 + children.Sum();
}
