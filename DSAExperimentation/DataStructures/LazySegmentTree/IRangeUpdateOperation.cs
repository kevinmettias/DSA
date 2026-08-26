namespace DSAExperimentation.DataStructures.LazySegmentTree;

// The algebra lazy propagation needs: Identity/Combine are the same associativity law
// SegmentTree.ICombineOperation<Element> states, independently redeclared here rather than inherited -
// SegmentTree and LazySegmentTree are different structure identities (different Representation,
// different Operations surface - see LazySegmentTree.cs), so per ARCHITECTURE.md §11.1's
// domain-separation precedent (IIndexedSequence redeclares rather than inherits
// IRandomAccessSequence for the same reason) this contract stands on its own.
//
// TUpdate is the "pending update" tag threaded through push-down:
//   - NoUpdate marks "nothing pending here."
//   - ComposeUpdate(outer, inner) folds a newly-arriving update on top of one already pending on
//     the same node - outer is the update being applied now, inner is what was already queued.
//   - ApplyUpdate(aggregate, update, rangeLength) applies one pending update to a node's
//     already-combined aggregate, given how many leaves that aggregate covers - range-add-then-
//     range-sum needs rangeLength (adding d to k elements changes their sum by d*k);
//     range-assign-then-range-max doesn't (assigning v to k elements makes their max v
//     regardless of k), but the shape covers both.
internal interface IRangeUpdateOperation<Element, TUpdate>
{
    static abstract Element Identity { get; }

    static abstract Element Combine(Element left, Element right);

    static abstract TUpdate NoUpdate { get; }

    static abstract TUpdate ComposeUpdate(TUpdate outer, TUpdate inner);

    static abstract Element ApplyUpdate(Element aggregate, TUpdate update, int rangeLength);
}
