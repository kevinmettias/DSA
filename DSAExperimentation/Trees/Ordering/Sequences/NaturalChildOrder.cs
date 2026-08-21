namespace DSAExperimentation.Trees;

public readonly struct NaturalChildOrder<TNode> : IChildOrder<TNode>
{
    public static IEnumerable<TNode> Apply(IEnumerable<TNode> children)
        => children;
}




