namespace DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

internal readonly struct ReverseChildOrder<TNode, TChildren>
    : IChildOrder<TNode, TChildren, ReversedChildren<TNode, TChildren>>
    where TChildren : struct, IChildren<TNode>
{
    public static ReversedChildren<TNode, TChildren> Apply(TChildren children) => new(children);
}
