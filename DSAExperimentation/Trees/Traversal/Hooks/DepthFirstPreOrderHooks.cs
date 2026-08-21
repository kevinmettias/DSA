namespace DSAExperimentation.Trees;

public readonly struct DepthFirstPreOrderHooks<TNode, TAction> : IDepthFirstHooks<TNode>
    where TAction : struct, INodeAction<TNode>
{
    public static void Enter(TNode node)
        => TAction.Invoke(node);

    public static void Exit(TNode node)
    {
    }
}




