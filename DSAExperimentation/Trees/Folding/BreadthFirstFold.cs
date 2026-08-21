namespace DSAExperimentation.Trees;

public static class BreadthFirstFold
{
    public static TState Fold<TNode, TTopology, TOrder, TAlgebra, TState>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
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

            foreach (var child in TOrder.Apply(TTopology.GetChildren(node)))
            {
                queue.Enqueue((child, depth + 1));
            }
        }

        return state;
    }
}




