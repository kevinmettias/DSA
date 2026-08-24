namespace DSAExperimentation.Graph;

public readonly struct NaturalChildOrder<TNode, TChildren> : IChildOrder<TNode, TChildren, TChildren>
    where TChildren : struct, IChildren<TNode>
{
    public static TChildren Apply(TChildren children) => children;
}
