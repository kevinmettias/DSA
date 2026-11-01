namespace DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

// A reversed view computed by index arithmetic over an existing IChildren, rather
// than by copying - this is what lets ReverseChildOrder be allocation-free.
/// <summary>Another children view read last to first, by index arithmetic rather than a copy.</summary>
/// <param name="inner">The children to present in reverse order.</param>
public readonly struct ReversedChildren<TNode, TChildren>(TChildren inner) : IChildren<TNode>
    where TChildren : struct, IChildren<TNode>
{
    public int Count => inner.Count;

    public TNode Get(int index) => inner.Get(inner.Count - 1 - index);
}
