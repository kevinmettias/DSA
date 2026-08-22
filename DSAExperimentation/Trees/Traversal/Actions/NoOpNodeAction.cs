namespace DSAExperimentation.Trees;

public readonly struct NoOpNodeAction<TNode> : INodeAction<TNode>
{
    public static void Invoke(TNode node)
    {
    }
}
