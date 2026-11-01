namespace DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

/// <summary>The child order that takes children last to first, through a view rather than a copy.</summary>
public readonly struct ReverseChildOrder<TNode, TChildren>
    : IChildOrder<TNode, TChildren, ReversedChildren<TNode, TChildren>>
    where TChildren : struct, IChildren<TNode>
{
    public static ReversedChildren<TNode, TChildren> Apply(TChildren children) => new(children);
}
