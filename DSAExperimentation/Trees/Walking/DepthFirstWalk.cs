namespace DSAExperimentation.Trees;

internal static class DepthFirstWalk
{
    internal static TResult Walk<TNode, TChildren, TStep, TResult>(
        TNode? root)
        where TNode : class
        where TChildren : struct, IChildEnumerationStrategy<TNode>
        where TStep : struct, IDepthFirstFoldAlgebra<TNode, TResult>
    {
        if (root is null)
        {
            return TStep.Empty;
        }

        TStep.Enter(root);

        if (TStep.CollectsChildResults)
        {
            var childResults = new List<TResult>();

            foreach (var child in TChildren.GetChildren(root))
            {
                childResults.Add(Walk<TNode, TChildren, TStep, TResult>(child));
            }

            return TStep.Combine(root, childResults);
        }

        foreach (var child in TChildren.GetChildren(root))
        {
            Walk<TNode, TChildren, TStep, TResult>(child);
        }

        return TStep.Combine(root, []);
    }
}
