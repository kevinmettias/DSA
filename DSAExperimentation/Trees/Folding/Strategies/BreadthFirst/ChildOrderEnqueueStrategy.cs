namespace DSAExperimentation.Trees;

internal readonly struct ChildOrderEnqueueStrategy<TNode, TTopology, TOrder>
    : IChildEnqueueStrategy<TNode>
    where TNode : class
    where TTopology : struct, ITreeTopology<TNode>
    where TOrder : struct, IChildOrder<TNode>
{
    public static void EnqueueChildren(
        Queue<TraversalFrame<TNode>> queue,
        TNode node,
        int depth)
    {
        foreach (var child in TOrder.Apply(TTopology.GetChildren(node)))
        {
            queue.Enqueue(new(child, depth + 1));
        }
    }
}
