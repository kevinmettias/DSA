namespace DSAExperimentation.Trees;

public static class BinaryBreadthFirstFold
{
    public static TState Fold<TNode, TTopology, TSchedule, TAlgebra, TState>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
        where TAlgebra : struct, IBreadthFirstFoldAlgebra<TNode, TState>
    {
        var state = TAlgebra.Seed;

        if (root is null)

        {
            return state;

        }

        var queue = new Queue<(TNode Node, int Depth)>();
        queue.Enqueue((root, 0));

        while (queue.Count > 0)
        {
            var (node, depth) = queue.Dequeue();
            state = TAlgebra.Accumulate(state, node, depth);

            EnqueueChildren<TNode, TTopology, TSchedule>(queue, node, depth);
        }

        return state;
    }

    private static void EnqueueChildren<TNode, TTopology, TSchedule>(
        Queue<(TNode Node, int Depth)> queue,
        TNode node,
        int depth)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
    {
        EnqueueIfPresent<TNode, TTopology, TSchedule>(
            queue,
            TSchedule.GetFirst<TNode, TTopology>(node),
            depth + 1);

        EnqueueIfPresent<TNode, TTopology, TSchedule>(
            queue,
            TSchedule.GetSecond<TNode, TTopology>(node),
            depth + 1);
    }

    private static void EnqueueIfPresent<TNode, TTopology, TSchedule>(
        Queue<(TNode Node, int Depth)> queue,
        TNode? node,
        int depth)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
    {
        if (node is null)
        {
            return;
        }

        queue.Enqueue((node, depth));
    }
}




