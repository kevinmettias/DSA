namespace DSAExperimentation.Trees;

internal interface IChildEnqueueStrategy<TNode>
    where TNode : class
{
    static abstract void EnqueueChildren(
        Queue<TraversalFrame<TNode>> queue,
        TNode node,
        int depth);
}
