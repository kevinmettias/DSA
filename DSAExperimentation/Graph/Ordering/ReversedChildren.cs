namespace DSAExperimentation.Graph;

// A reversed view computed by index arithmetic over an existing IChildren, rather
// than by copying - this is what lets ReverseChildOrder be allocation-free.
public readonly struct ReversedChildren<TNode, TChildren>(TChildren inner) : IChildren<TNode>
    where TChildren : struct, IChildren<TNode>
{
    public int Count => inner.Count;

    public TNode this[int index] => inner[inner.Count - 1 - index];
}
