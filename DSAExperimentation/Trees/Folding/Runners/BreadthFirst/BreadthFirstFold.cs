namespace DSAExperimentation.Trees;

public static class BreadthFirstFold
{
    public static TState Fold<TNode, TTopology, TOrder, TAlgebra, TState>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAlgebra : struct, IBreadthFirstFoldAlgebra<TNode, TState>
        => Fold<
            TNode,
            ChildOrderEnqueueStrategy<TNode, TTopology, TOrder>,
            TAlgebra,
            TState>(root);

    internal static TState Fold<TNode, TChildEnqueue, TAlgebra, TState>(
        TNode? root)
        where TNode : class
        where TChildEnqueue : struct, IChildEnqueueStrategy<TNode>
        where TAlgebra : struct, IBreadthFirstFoldAlgebra<TNode, TState>
    {
        var state = TAlgebra.Seed;

        if (root is null)
        {
            return state;
        }

        var queue = new Queue<TraversalFrame<TNode>>();
        queue.Enqueue(new(root, 0));

        while (queue.Count > 0)
        {
            var (node, depth) = queue.Dequeue();
            state = TAlgebra.Accumulate(state, node, depth);

            TChildEnqueue.EnqueueChildren(queue, node, depth);
        }

        return state;
    }
}
