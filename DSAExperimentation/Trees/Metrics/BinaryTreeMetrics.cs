namespace DSAExperimentation.Trees;

public static class BinaryTreeMetrics
{
    public static int Height<TNode, TTopology, TSchedule>(TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
        => BinaryDepthFirstFold.Fold<
            TNode,
            TTopology,
            TSchedule,
            HeightAlgebra<TNode>,
            int>(root);

    public static int Size<TNode, TTopology, TSchedule>(TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
        => BinaryDepthFirstFold.Fold<
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
        var state = BinaryDepthFirstFold.Fold<
            TNode,
            TTopology,
            TSchedule,
            DiameterAlgebra<TNode>,
            HeightDiameterState>(root);

        return state.Diameter;
    }

    public static FoldResultPair<int, int> HeightAndSize<TNode, TTopology, TSchedule>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
        => BinaryDepthFirstFold.Fold<
            TNode,
            TTopology,
            TSchedule,
            ZipBinaryDepthFirstAlgebra<
                TNode,
                int,
                int,
                HeightAlgebra<TNode>,
                SizeAlgebra<TNode>>,
            FoldResultPair<int, int>>(root);
}




