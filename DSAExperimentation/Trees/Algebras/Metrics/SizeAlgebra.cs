namespace DSAExperimentation.Trees;

public readonly struct SizeAlgebra<TNode>
    : IBinaryFoldAlgebra<TNode, int>
{
    public static int Empty
        => 0;

    public static int Combine(
        TNode node,
        BinaryChildren<int> children)
        => 1 + children.Left + children.Right;
}




