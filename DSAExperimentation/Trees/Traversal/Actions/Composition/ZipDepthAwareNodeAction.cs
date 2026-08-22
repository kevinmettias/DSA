namespace DSAExperimentation.Trees;

public readonly struct ZipDepthAwareNodeAction<TNode, TFirst, TSecond> : IDepthAwareNodeAction<TNode>
    where TFirst : struct, IDepthAwareNodeAction<TNode>
    where TSecond : struct, IDepthAwareNodeAction<TNode>
{
    public static void Invoke(TNode node, int depth)
    {
        TFirst.Invoke(node, depth);
        TSecond.Invoke(node, depth);
    }
}
