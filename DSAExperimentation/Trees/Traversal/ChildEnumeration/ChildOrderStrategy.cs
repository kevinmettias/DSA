namespace DSAExperimentation.Trees;

internal readonly struct ChildOrderStrategy<TNode, TTopology, TOrder>
    : IChildEnumerationStrategy<TNode>
    where TNode : class
    where TTopology : struct, ITreeTopology<TNode>
    where TOrder : struct, IChildOrder<TNode>
{
    public static IEnumerable<TNode> GetChildren(TNode node)
        => TOrder.Apply(TTopology.GetChildren(node));
}
