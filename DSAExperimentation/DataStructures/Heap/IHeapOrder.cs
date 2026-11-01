namespace DSAExperimentation.DataStructures.Heap;

// The single axis Heap<Element,TOrder> varies on: whether a candidate outranks the value it would
// replace at the root. Unlike IGraphTopology<TNode,TChildren>, this carries no representation
// type parameter - a heap's "children" are index arithmetic over its own backing array (see
// HeapArrayIndex), not an independently-pluggable collection, so there's nothing for a second
// type parameter to vary independently of.
/// <summary>The priority rule a <see cref="Heap{Element, TOrder}"/> orders its elements by.</summary>
public interface IHeapOrder<Element>
{
    /// <summary>Reports whether a candidate outranks an incumbent and so belongs nearer the root.</summary>
    /// <param name="candidate">The element that would move nearer the root.</param>
    /// <param name="incumbent">The element it is ranked against.</param>
    /// <returns>true when the candidate outranks the incumbent; otherwise false.</returns>
    static abstract bool HasPriority(Element candidate, Element incumbent);
}
