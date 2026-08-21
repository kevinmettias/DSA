namespace DSAExperimentation.Trees;

public readonly struct DepthFirstPostOrderHooks<TNode, TAction> : IDepthFirstHooks<TNode>
    where TAction : struct, INodeAction<TNode>
{
    public static void Enter(TNode node)
    {
    }

    public static void Exit(TNode node)
        => TAction.Invoke(node);
}




