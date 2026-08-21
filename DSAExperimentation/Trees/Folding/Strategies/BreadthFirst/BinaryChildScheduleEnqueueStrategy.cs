namespace DSAExperimentation.Trees;

internal readonly struct BinaryChildScheduleEnqueueStrategy<TNode, TTopology, TSchedule>
    : IChildEnqueueStrategy<TNode>
    where TNode : class
    where TTopology : struct, IBinaryTreeTopology<TNode>
    where TSchedule : struct, IBinaryChildSchedule
{
    public static void EnqueueChildren(
        Queue<TraversalFrame<TNode>> queue,
        TNode node,
        int depth)
    {
        EnqueueIfPresent(
            queue,
            TSchedule.GetFirst<TNode, TTopology>(node),
            depth + 1);

        EnqueueIfPresent(
            queue,
            TSchedule.GetSecond<TNode, TTopology>(node),
            depth + 1);
    }

    private static void EnqueueIfPresent(
        Queue<TraversalFrame<TNode>> queue,
        TNode? node,
        int depth)
    {
        if (node is null)
        {
            return;
        }

        queue.Enqueue(new(node, depth));
    }
}
