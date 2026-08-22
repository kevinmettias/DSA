namespace DSAExperimentation.Trees;

public readonly struct ZipNodeAction<TNode, TFirst, TSecond> : INodeAction<TNode>
    where TFirst : struct, INodeAction<TNode>
    where TSecond : struct, INodeAction<TNode>
{
    public static void Invoke(TNode node)
    {
        TFirst.Invoke(node);
        TSecond.Invoke(node);
    }
}
