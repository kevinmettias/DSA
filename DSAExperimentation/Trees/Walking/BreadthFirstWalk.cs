namespace DSAExperimentation.Trees;

internal static class BreadthFirstWalk
{
    internal static TState Walk<TNode, TChildren, TStep, TState>(
        TNode? root)
        where TNode : class
        where TChildren : struct, IChildEnumerationStrategy<TNode>
        where TStep : struct, IBreadthFirstReduceAlgebra<TNode, TState>
    {
        var state = TStep.Seed;

        if (root is null)
        {
            return state;
        }

        var queue = new Queue<TraversalFrame<TNode>>();
        EnqueueRoot<TNode, TStep, TState>(queue, root);

        return VisitQueuedNodes<TNode, TChildren, TStep, TState>(queue, state);
    }

    private static void EnqueueRoot<TNode, TStep, TState>(
        Queue<TraversalFrame<TNode>> queue,
        TNode root)
        where TNode : class
        where TStep : struct, IBreadthFirstReduceAlgebra<TNode, TState>
    {
        TStep.Discover(root, 0);
        queue.Enqueue(new(root, 0));
    }

    private static TState VisitQueuedNodes<TNode, TChildren, TStep, TState>(
        Queue<TraversalFrame<TNode>> queue,
        TState state)
        where TNode : class
        where TChildren : struct, IChildEnumerationStrategy<TNode>
        where TStep : struct, IBreadthFirstReduceAlgebra<TNode, TState>
    {
        while (queue.Count > 0)
        {
            var (node, depth) = queue.Dequeue();
            state = TStep.Accumulate(state, node, depth);
            EnqueueChildren<TNode, TChildren, TStep, TState>(queue, node, depth);
        }

        TStep.Finish();

        return state;
    }

    private static void EnqueueChildren<TNode, TChildren, TStep, TState>(
        Queue<TraversalFrame<TNode>> queue,
        TNode node,
        int depth)
        where TNode : class
        where TChildren : struct, IChildEnumerationStrategy<TNode>
        where TStep : struct, IBreadthFirstReduceAlgebra<TNode, TState>
    {
        foreach (var child in TChildren.GetChildren(node))
        {
            TStep.Discover(child, depth + 1);
            queue.Enqueue(new(child, depth + 1));
        }
    }
}
