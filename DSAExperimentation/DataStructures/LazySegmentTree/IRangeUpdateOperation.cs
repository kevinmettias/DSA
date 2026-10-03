using DSAExperimentation.DataStructures.ElementAlgebra;

namespace DSAExperimentation.DataStructures.LazySegmentTree;

// The algebra lazy propagation needs: the element monoid every range structure shares
// (ElementAlgebra.ICombineOperation - its Identity and Combine), refined with an update action that
// only this structure has. That refinement is structure-local by design: a member one structure needs
// stays out of the shared chain (ARCHITECTURE.md §11.4), so SegmentTree and FenwickTree never see it.
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
internal interface IRangeUpdateOperation<Element, TUpdate> : ICombineOperation<Element>
{
    static abstract TUpdate NoUpdate { get; }

    static abstract TUpdate ComposeUpdate(TUpdate outer, TUpdate inner);

    static abstract Element ApplyUpdate(Element aggregate, TUpdate update, int rangeLength);
}
