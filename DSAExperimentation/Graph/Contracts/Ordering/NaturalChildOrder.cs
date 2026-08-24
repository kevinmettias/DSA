namespace DSAExperimentation.Graph.Contracts.Ordering;

internal readonly struct NaturalChildOrder<TNode, TChildren> : IChildOrder<TNode, TChildren, TChildren>
    where TChildren : struct, IChildren<TNode>
{
    public static TChildren Apply(TChildren children) => children;
}
