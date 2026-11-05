using DSAExperimentation.Domain.Mathematics.Algebra;

namespace DSAExperimentation.DataStructures.LazySegmentTree;

// The algebra lazy propagation needs: the element monoid every range structure shares
// (ElementAlgebra.IMonoid - its Identity and Combine), refined with an update action that
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
/// <summary>
/// The algebra LazySegmentTree runs on: an element monoid plus updates it can defer, compose and apply.
/// </summary>
public interface IRangeUpdateOperation<Element, TUpdate> : IMonoid<Element>
{
    /// <summary>The update meaning nothing is pending, distinct from every update a caller applies.</summary>
    static abstract TUpdate NoUpdate { get; }

    /// <summary>Folds an arriving update onto one already pending on the same node.</summary>
    /// <param name="outer">The update arriving now.</param>
    /// <param name="inner">The update already pending, or <see cref="NoUpdate"/> when nothing is.</param>
    /// <returns>One update equivalent to applying <paramref name="inner"/> then <paramref name="outer"/>.</returns>
    static abstract TUpdate ComposeUpdate(TUpdate outer, TUpdate inner);

    /// <summary>Applies an update to the combined value of a node's whole range.</summary>
    /// <param name="aggregate">The combined value of the range before the update.</param>
    /// <param name="update">The update to apply to every element of the range.</param>
    /// <param name="rangeLength">How many elements the range covers.</param>
    /// <returns>The range's combined value after the update is applied to each of its elements.</returns>
    static abstract Element ApplyUpdate(Element aggregate, TUpdate update, int rangeLength);
}
