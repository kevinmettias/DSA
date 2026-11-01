namespace DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

/// <summary>The child order that keeps children in the order their topology gives them.</summary>
public readonly struct NaturalChildOrder<TNode, TChildren> : IChildOrder<TNode, TChildren, TChildren>
    where TChildren : struct, IChildren<TNode>
{
    public static TChildren Apply(TChildren children) => children;
}
