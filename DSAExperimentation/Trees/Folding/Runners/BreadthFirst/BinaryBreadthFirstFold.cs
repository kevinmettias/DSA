namespace DSAExperimentation.Trees;

public static class BinaryBreadthFirstFold
{
    public static TState Fold<TNode, TTopology, TSchedule, TAlgebra, TState>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
        where TAlgebra : struct, IBreadthFirstFoldAlgebra<TNode, TState>
        => BreadthFirstFold.Fold<
            TNode,
            BinaryChildScheduleEnqueueStrategy<TNode, TTopology, TSchedule>,
            TAlgebra,
            TState>(root);
}
