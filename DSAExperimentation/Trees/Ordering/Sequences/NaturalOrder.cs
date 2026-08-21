namespace DSAExperimentation.Trees;

public readonly struct NaturalOrder<TNode> : IChildOrder<TNode>
{
    public static IEnumerable<TNode> Apply(IEnumerable<TNode> children)
        => children;
}
