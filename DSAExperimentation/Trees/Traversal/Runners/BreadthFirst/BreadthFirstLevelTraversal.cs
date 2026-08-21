namespace DSAExperimentation.Trees;

public static class BreadthFirstLevelTraversal
{
    public static void Traverse<TNode, TTopology, TOrder, TAction>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAction : struct, ILevelVisitAction<TNode>
        => BreadthFirstHookTraversal.Traverse<
            TNode,
            TTopology,
            TOrder,
            BreadthFirstLevelVisitHooks<TNode, TAction>>(root);
}
