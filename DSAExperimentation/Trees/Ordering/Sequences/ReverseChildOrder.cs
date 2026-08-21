namespace DSAExperimentation.Trees;

public readonly struct ReverseChildOrder<TNode> : IChildOrder<TNode>
{
    public static IEnumerable<TNode> Apply(IEnumerable<TNode> children)
        => children.Reverse();
}




