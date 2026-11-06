namespace DSAExperimentation.DataStructures.SegmentTree;

// The second axis of the one segment tree (ARCHITECTURE.md §13.10): how an update to a whole
// range acts on the element monoid's combined values, beside the monoid itself. The members were
// IRangeUpdateOperation's while the lazy tree was a structure of its own; they stand apart now so
// the trivial action, NoAction, can be a witness of this alone, with no monoid to forward.
//
// TUpdate is the "pending update" tag threaded through push-down:
//   - NoUpdate marks "nothing pending here."
//   - ComposeUpdate(outer, inner) folds a newly-arriving update on top of one already pending on
//     the same node - outer is the update being applied now, inner is what was already queued.
//   - ApplyUpdate(aggregate, update, rangeLength) applies one pending update to a node's
//     already-combined aggregate, given how many leaves that aggregate covers - range-add-then-
//     range-sum needs rangeLength (adding d to k elements changes their sum by d*k);
//     range-assign-then-range-max doesn't (assigning v to k elements makes their max v regardless
//     of k), but the shape covers both.
// It stays structure-local, beside the tree, for §11.4's reason: no other range structure defers.
/// <summary>
/// Updates a segment tree can defer over a range, compose with each other, and apply to a range's
/// value.
/// </summary>
/// <typeparam name="Element">
/// The tree's element type, whose combined values an update acts on.
/// </typeparam>
/// <typeparam name="TUpdate">
/// The type of one update, also the tag a node keeps while it is pending.
/// </typeparam>
public interface IRangeAction<Element, TUpdate>
{
    /// <summary>
    /// The update meaning nothing is pending, distinct from every update a caller applies.
    /// </summary>
    static abstract TUpdate NoUpdate { get; }

    /// <summary>Folds an arriving update onto one already pending on the same node.</summary>
    /// <param name="outer">The update arriving now.</param>
    /// <param name="inner">
    /// The update already pending, or <see cref="NoUpdate"/> when nothing is.
    /// </param>
    /// <returns>
    /// One update equivalent to applying <paramref name="inner"/> then <paramref name="outer"/>.
    /// </returns>
    static abstract TUpdate ComposeUpdate(TUpdate outer, TUpdate inner);

    /// <summary>Applies an update to the combined value of a node's whole range.</summary>
    /// <param name="aggregate">The combined value of the range before the update.</param>
    /// <param name="update">The update to apply to every element of the range.</param>
    /// <param name="rangeLength">How many elements the range covers.</param>
    /// <returns>
    /// The range's combined value after the update is applied to each of its elements.
    /// </returns>
    static abstract Element ApplyUpdate(Element aggregate, TUpdate update, int rangeLength);
}
