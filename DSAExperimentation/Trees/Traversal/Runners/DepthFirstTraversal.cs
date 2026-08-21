namespace DSAExperimentation.Trees;

public static class DepthFirstTraversal
{
    public static void Traverse<TNode, TTopology, TOrder, THooks>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where THooks : struct, IDepthFirstHooks<TNode>
    {
        if (root is null)
            return;

        THooks.Enter(root);

        foreach (var child in TOrder.Apply(TTopology.GetChildren(root)))
        {
            Traverse<TNode, TTopology, TOrder, THooks>(child);
        }

        THooks.Exit(root);
    }
}




