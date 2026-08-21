namespace DSAExperimentation.Trees;

public static class BreadthFirstTraversal
{
    public static void Traverse<TNode, TTopology, TOrder, TAction>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAction : struct, INodeAction<TNode>
    {
        if (root is null)
            return;

        var queue = new Queue<TNode>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            TAction.Invoke(node);

            foreach (var child in TOrder.Apply(TTopology.GetChildren(node)))
            {
                queue.Enqueue(child);
            }
        }
    }
}
