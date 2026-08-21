namespace DSAExperimentation.Trees;

public readonly struct HeightAlgebra<TNode>
    : IBinaryDepthFirstFoldAlgebra<TNode, int>
{
    public static int Empty
        => 0;

    public static int Combine(
        TNode node,
        BinaryChildren<int> children)
        => 1 + Math.Max(children.Left, children.Right);
}




