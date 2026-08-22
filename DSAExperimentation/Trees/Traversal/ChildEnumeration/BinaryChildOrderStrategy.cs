namespace DSAExperimentation.Trees;

internal readonly struct BinaryChildOrderStrategy<TNode, TTopology, TOrder>
    : IChildEnumerationStrategy<TNode>
    where TNode : class
    where TTopology : struct, IBinaryTreeTopology<TNode>
    where TOrder : struct, IBinaryChildOrder
{
    public static IEnumerable<TNode> GetChildren(TNode node)
    {
        var first = TOrder.GetFirst<TNode, TTopology>(node);

        if (first is not null)
        {
            yield return first;
        }

        var second = TOrder.GetSecond<TNode, TTopology>(node);

        if (second is not null)
        {
            yield return second;
        }
    }
}
