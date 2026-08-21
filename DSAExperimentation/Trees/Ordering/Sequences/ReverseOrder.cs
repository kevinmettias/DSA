namespace DSAExperimentation.Trees;

public readonly struct ReverseOrder<TNode> : IChildOrder<TNode>
{
    public static IEnumerable<TNode> Apply(IEnumerable<TNode> children)
        => children.Reverse();
}
