namespace DSAExperimentation.DataStructures.MonotonicDeque;

// The one axis MonotonicDeque varies on: whether a resident key is dominated by an arriving one, so
// that the resident can never be the window's extremum again - it is older, and no better. It is a
// Topology witness by §5's first row: Push re-derives it at every back comparison, and maximum versus
// minimum changes which key the front reports (§10.1's test). The set is closed to the two orders
// this library enumerates; a tuple key or a negated key only projects that same choice, as
// ByPriorityOrder does for IHeapOrder.
//
// Dominance includes ties: a resident equal to the arriving key is dominated, so among equal extrema
// the front reports the most recent position, the one that will stay in the window longest. Every
// solution that hand-rolled this deque evicted ties the same way. A "keep ties" order would be a
// third witness.
//
// Neither IHeapOrder, which is Heap's own Topology contract (§5 step 5), nor ElementAlgebra's
// MaxOperation/MinOperation: a monoid does not promise that Combine returns one of its arguments, so a
// deque over SumOperation would compile and mean nothing.
internal interface IWindowOrder<Key>
{
    static abstract bool IsDominatedBy(Key resident, Key arriving);
}
