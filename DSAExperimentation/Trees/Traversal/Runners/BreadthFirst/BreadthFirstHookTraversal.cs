namespace DSAExperimentation.Trees;

public static class BreadthFirstHookTraversal
{
    public static void Traverse<TNode, TTopology, TOrder, THooks>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
    {
        if (root is null)
        {
            return;
        }

        var queue = new Queue<(TNode Node, int Depth)>();
        THooks.Discover(root, 0);
        queue.Enqueue((root, 0));

        while (queue.Count > 0)
        {
            var (node, depth) = queue.Dequeue();

            THooks.Visit(node, depth);

            DiscoverChildren<TNode, TTopology, TOrder, THooks>(
                queue,
                node,
                depth);
        }

        THooks.Finish();
    }

    private static void DiscoverChildren<TNode, TTopology, TOrder, THooks>(
        Queue<(TNode Node, int Depth)> queue,
        TNode node,
        int depth)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
    {
        foreach (var child in TOrder.Apply(TTopology.GetChildren(node)))
        {
            THooks.Discover(child, depth + 1);
            queue.Enqueue((child, depth + 1));
        }
    }
}
