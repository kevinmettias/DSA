namespace DSAExperimentation.Trees;

public readonly struct LevelGroupedBreadthFirstTraversal<TNode, TTopology, TOrder, TAction>
    : ITreeTraversal<TNode>
    where TNode : class
    where TTopology : struct, ITreeTopology<TNode>
    where TOrder : struct, IChildOrder<TNode>
    where TAction : struct, ILevelVisitAction<TNode>
{
    public static void Traverse(TNode? root)
        => BreadthFirstTraversal<
            TNode,
            TTopology,
            TOrder,
            LevelGroupedBreadthFirstHooks<TNode, TAction>>.Traverse(root);
}
