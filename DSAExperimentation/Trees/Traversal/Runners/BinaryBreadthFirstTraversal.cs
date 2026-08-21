namespace DSAExperimentation.Trees;

public static class BinaryBreadthFirstTraversal
{
    public static void Traverse<TNode, TTopology, TSchedule, TAction>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
        where TAction : struct, INodeAction<TNode>
        => TraverseWithHooks<
            TNode,
            TTopology,
            TSchedule,
            BreadthFirstNodeVisitHooks<TNode, TAction>>(root);

    public static void TraverseWithDepth<TNode, TTopology, TSchedule, TAction>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
        where TAction : struct, IDepthAwareNodeAction<TNode>
        => TraverseWithHooks<
            TNode,
            TTopology,
            TSchedule,
            BreadthFirstDepthAwareNodeVisitHooks<TNode, TAction>>(root);

    public static void TraverseByLevel<TNode, TTopology, TSchedule, TAction>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
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
                AddNextLevelNode<TNode, TTopology, TSchedule>(
                    queue,
                    level);
            }

            TAction.Invoke(level, depth);
            depth++;
        }
    }

    public static void TraverseWithHooks<TNode, TTopology, TSchedule, THooks>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
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

            DiscoverChildren<TNode, TTopology, TSchedule, THooks>(
                queue,
                node,
                depth);
        }
    }

    private static void AddNextLevelNode<TNode, TTopology, TSchedule>(
        Queue<TNode> queue,
        List<TNode> level)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
    {
        var node = queue.Dequeue();
        level.Add(node);

        EnqueueIfPresent<TNode, TTopology, TSchedule>(
            queue,
            TSchedule.GetFirst<TNode, TTopology>(node));

        EnqueueIfPresent<TNode, TTopology, TSchedule>(
            queue,
            TSchedule.GetSecond<TNode, TTopology>(node));
    }

    private static void DiscoverChildren<TNode, TTopology, TSchedule, THooks>(
        Queue<(TNode Node, int Depth)> queue,
        TNode node,
        int depth)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
        where THooks : struct, IBreadthFirstHooks<TNode>
    {
        EnqueueIfPresent<TNode, TTopology, TSchedule, THooks>(
            queue,
            TSchedule.GetFirst<TNode, TTopology>(node),
            depth + 1);

        EnqueueIfPresent<TNode, TTopology, TSchedule, THooks>(
            queue,
            TSchedule.GetSecond<TNode, TTopology>(node),
            depth + 1);
    }

    private static void EnqueueIfPresent<TNode, TTopology, TSchedule>(
        Queue<TNode> queue,
        TNode? node)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
    {
        if (node is null)
        {
            return;
        }

        queue.Enqueue(node);
    }

    private static void EnqueueIfPresent<TNode, TTopology, TSchedule, THooks>(
        Queue<(TNode Node, int Depth)> queue,
        TNode? node,
        int depth)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
        where THooks : struct, IBreadthFirstHooks<TNode>
    {
        if (node is null)
        {
            return;
        }

        THooks.Discover(node, depth);
        queue.Enqueue((node, depth));
    }
}




