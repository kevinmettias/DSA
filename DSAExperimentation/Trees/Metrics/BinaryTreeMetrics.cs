namespace DSAExperimentation.Trees;

public static class BinaryTreeMetrics
{
    public static int Height<TNode, TTopology, TOrder>(TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TOrder : struct, IBinaryChildOrder
        => BinaryTreeFold.Fold<
            TNode,
            TTopology,
            TOrder,
            HeightAlgebra<TNode>,
            int>(root);

    public static int Size<TNode, TTopology, TOrder>(TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TOrder : struct, IBinaryChildOrder
        => BinaryTreeFold.Fold<
            TNode,
            TTopology,
            TOrder,
            SizeAlgebra<TNode>,
            int>(root);

    public static int Diameter<TNode, TTopology, TOrder>(TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TOrder : struct, IBinaryChildOrder
    {
        var state = BinaryTreeFold.Fold<
            TNode,
            TTopology,
            TOrder,
            DiameterAlgebra<TNode>,
            HeightDiameterState>(root);

        return state.Diameter;
    }

    public static FoldResultPair<int, int> HeightAndSize<TNode, TTopology, TOrder>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TOrder : struct, IBinaryChildOrder
        => BinaryTreeFold.Fold<
            TNode,
            TTopology,
            TOrder,
            ZipBinaryFoldAlgebra<
                TNode,
                int,
                int,
                HeightAlgebra<TNode>,
                SizeAlgebra<TNode>>,
            FoldResultPair<int, int>>(root);
}




