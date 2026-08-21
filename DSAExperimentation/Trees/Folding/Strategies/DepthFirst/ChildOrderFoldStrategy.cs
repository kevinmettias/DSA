namespace DSAExperimentation.Trees;

internal readonly struct ChildOrderFoldStrategy<TNode, TTopology, TOrder, TAlgebra, TResult>
    : IFoldStrategy<TNode, TResult>
    where TNode : class
    where TTopology : struct, ITreeTopology<TNode>
    where TOrder : struct, IChildOrder<TNode>
    where TAlgebra : struct, IDepthFirstFoldAlgebra<TNode, TResult>
{
    public static TResult Empty => TAlgebra.Empty;

    public static TResult FoldNode(TNode node)
    {
        var childResults = new List<TResult>();

        foreach (var child in TOrder.Apply(TTopology.GetChildren(node)))
        {
            childResults.Add(
                DepthFirstFold.Fold<
                    TNode,
                    ChildOrderFoldStrategy<TNode, TTopology, TOrder, TAlgebra, TResult>,
                    TResult>(child));
        }

        return TAlgebra.Combine(node, childResults);
    }
}
