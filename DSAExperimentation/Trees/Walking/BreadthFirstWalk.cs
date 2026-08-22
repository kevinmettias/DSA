namespace DSAExperimentation.Trees;

internal static class BreadthFirstWalk
{
    internal static TState Walk<TNode, TChildren, TStep, TState>(
        TNode? root)
        where TNode : class
        where TChildren : struct, IChildEnumerationStrategy<TNode>
        where TStep : struct, IBreadthFirstFoldAlgebra<TNode, TState>
    {
        var state = TStep.Seed;

        if (root is null)
        {
            return state;
        }

        TStep.Discover(root, 0);

        var queue = new Queue<TraversalFrame<TNode>>();
        queue.Enqueue(new(root, 0));

        while (queue.Count > 0)
        {
            var (node, depth) = queue.Dequeue();
            state = TStep.Accumulate(state, node, depth);

            foreach (var child in TChildren.GetChildren(node))
            {
                TStep.Discover(child, depth + 1);
                queue.Enqueue(new(child, depth + 1));
            }
        }

        TStep.Finish();

        return state;
    }
}
