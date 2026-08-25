namespace DSAExperimentation.DataStructures.Heap;

// The single axis Heap<T,TOrder> varies on: whether a candidate outranks the value it would
// replace at the root. Unlike IGraphTopology<TNode,TChildren>, this carries no representation
// type parameter - a heap's "children" are index arithmetic over its own backing array (see
// HeapArrayIndex), not an independently-pluggable collection, so there's nothing for a second
// type parameter to vary independently of.
internal interface IHeapOrder<T>
{
    static abstract bool HasPriority(T candidate, T incumbent);
}
