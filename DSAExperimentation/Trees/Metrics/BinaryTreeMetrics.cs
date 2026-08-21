namespace DSAExperimentation.Trees;

public static class BinaryTreeMetrics
{
    public static int Height<TNode, TTopology, TSchedule>(TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
        => BinaryTreeFold.Fold<
            TNode,
            TTopology,
            TSchedule,
            HeightAlgebra<TNode>,
            int>(root);

    public static int Size<TNode, TTopology, TSchedule>(TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
        => BinaryTreeFold.Fold<
            TNode,
            TTopology,
            TSchedule,
            SizeAlgebra<TNode>,
            int>(root);

    public static int Diameter<TNode, TTopology, TSchedule>(TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
    {
        var state = BinaryTreeFold.Fold<
            TNode,
            TTopology,
            TSchedule,
            DiameterAlgebra<TNode>,
            HeightDiameterState>(root);

        return state.Diameter;
    }

    public static FoldPair<int, int> HeightAndSize<TNode, TTopology, TSchedule>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
        => BinaryTreeFold.Fold<
            TNode,
            TTopology,
            TSchedule,
            ZipBinaryAlgebra<
                TNode,
                int,
                int,
                HeightAlgebra<TNode>,
                SizeAlgebra<TNode>>,
            FoldPair<int, int>>(root);
}
