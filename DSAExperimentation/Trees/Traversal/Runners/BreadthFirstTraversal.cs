namespace DSAExperimentation.Trees;

public static class BreadthFirstTraversal
{
    public static void Traverse<TNode, TTopology, TOrder, TAction>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAction : struct, INodeAction<TNode>
        => TraverseWithHooks<
            TNode,
            TTopology,
            TOrder,
            BreadthFirstNodeVisitHooks<TNode, TAction>>(root);

    public static void TraverseWithDepth<TNode, TTopology, TOrder, TAction>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAction : struct, IDepthAwareNodeAction<TNode>
        => TraverseWithHooks<
            TNode,
            TTopology,
            TOrder,
            BreadthFirstDepthAwareNodeVisitHooks<TNode, TAction>>(root);

    public static void TraverseByLevel<TNode, TTopology, TOrder, TAction>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAction : struct, ILevelVisitAction<TNode>
    {
        if (root is null)
        {
            return;
        }

        var queue = new Queue<TNode>();
        queue.Enqueue(root);
        var depth = 0;

        while (queue.Count > 0)
        {
            var width = queue.Count;
            var level = new List<TNode>(width);

            for (var index = 0; index < width; index++)
            {
                AddNextLevelNode<TNode, TTopology, TOrder>(
                    queue,
                    level);
            }

            TAction.Invoke(level, depth);
            depth++;
        }
    }

    public static void TraverseWithHooks<TNode, TTopology, TOrder, THooks>(
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

            foreach (var child in TOrder.Apply(TTopology.GetChildren(node)))
            {
                THooks.Discover(child, depth + 1);
                queue.Enqueue((child, depth + 1));
            }
        }
    }

    private static void AddNextLevelNode<TNode, TTopology, TOrder>(
        Queue<TNode> queue,
        List<TNode> level)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
    {
        var node = queue.Dequeue();
        level.Add(node);

        foreach (var child in TOrder.Apply(TTopology.GetChildren(node)))
        {
            queue.Enqueue(child);
        }
    }
}
