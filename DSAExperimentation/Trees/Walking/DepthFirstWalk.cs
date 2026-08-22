namespace DSAExperimentation.Trees;

internal static class DepthFirstWalk
{
    internal static TResult Walk<TNode, TChildren, TStep, TResult>(
        TNode? root)
        where TNode : class
        where TChildren : struct, IChildEnumerationStrategy<TNode>
        where TStep : struct, IFoldAlgebra<TNode, TResult>
    {
        if (root is null)
        {
            return TStep.Empty;
        }

        return WalkNode<TNode, TChildren, TStep, TResult>(root, 0);
    }

    private static TResult WalkNode<TNode, TChildren, TStep, TResult>(
        TNode node,
        int depth)
        where TNode : class
        where TChildren : struct, IChildEnumerationStrategy<TNode>
        where TStep : struct, IFoldAlgebra<TNode, TResult>
    {
        TStep.Enter(node, depth);

        return TStep.CollectsChildResults
            ? WalkCollectingChildResults<TNode, TChildren, TStep, TResult>(node, depth)
            : WalkVisitingChildren<TNode, TChildren, TStep, TResult>(node, depth);
    }

    private static TResult WalkCollectingChildResults<TNode, TChildren, TStep, TResult>(
        TNode node,
        int depth)
        where TNode : class
        where TChildren : struct, IChildEnumerationStrategy<TNode>
        where TStep : struct, IFoldAlgebra<TNode, TResult>
    {
        var childResults = new List<TResult>();

        foreach (var child in TChildren.GetChildren(node))
        {
            childResults.Add(WalkNode<TNode, TChildren, TStep, TResult>(child, depth + 1));
        }

        return TStep.Combine(node, childResults);
    }

    private static TResult WalkVisitingChildren<TNode, TChildren, TStep, TResult>(
        TNode node,
        int depth)
        where TNode : class
        where TChildren : struct, IChildEnumerationStrategy<TNode>
        where TStep : struct, IFoldAlgebra<TNode, TResult>
    {
        foreach (var child in TChildren.GetChildren(node))
        {
            WalkNode<TNode, TChildren, TStep, TResult>(child, depth + 1);
        }

        return TStep.Combine(node, []);
    }
}