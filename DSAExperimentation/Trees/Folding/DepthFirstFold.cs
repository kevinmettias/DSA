namespace DSAExperimentation.Trees;

public static class DepthFirstFold
{
    public static TResult Fold<TNode, TTopology, TOrder, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAlgebra : struct, IDepthFirstFoldAlgebra<TNode, TResult>
    {
        if (root is null)
        {
            return TAlgebra.Empty;
        }

        var childResults = new List<TResult>();

        foreach (var child in TOrder.Apply(TTopology.GetChildren(root)))
        {
            childResults.Add(
                Fold<TNode, TTopology, TOrder, TAlgebra, TResult>(child));
        }

        return TAlgebra.Combine(root, childResults);
    }
}




