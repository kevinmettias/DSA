namespace DSAExperimentation.DataStructures.SegmentTree;

// The single axis SegmentTree<Element,TOperation> varies on: how two adjacent ranges' already-combined
// values merge into their parent's value. Must be associative - Combine(Combine(a,b),c) ==
// Combine(a,Combine(b,c)) - so that grouping ranges differently (as the tree's own shape forces)
// never changes the answer. Unenforced precondition law, the same shape as BinarySearch's
// sortedness: an operation that isn't actually associative still builds and queries without
// error, just silently returns a wrong combined value.
//
// Modeled as a static-abstract witness, not a runtime object like IComparer<T>: the discriminator
// is not whether the operation's content is open-ended (it is - sum, min, max, gcd, ... are not a
// closed set this library enumerates), it's whether the operation is a stateless pure function
// closed over at the generic-instantiation site rather than one that can carry closure-captured
// runtime state. IHeapOrder<Element>/IFoldAlgebra<TNode,TResult>/IPathHeuristic<TNode,TWeight> are the
// precedent - all just as open-ended as any comparer, all still witnesses for exactly this reason.
internal interface ICombineOperation<Element>
{
    static abstract Element Identity { get; }

    static abstract Element Combine(Element left, Element right);
}
