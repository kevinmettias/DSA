using DSAExperimentation.Domain.Mathematics.Algebra;

namespace DSAExperimentation.DataStructures.SegmentTree;

// An element monoid and an action on it, declared together in one witness: the action a range
// update takes depends on the monoid it acts on - adding d to k elements adds d*k to their sum but
// d to their maximum - so a witness that states both cannot be paired with the wrong monoid.
// SegmentTree<Element, TUpdate, TOperation> injects such a witness as both its monoid and its
// action (ARCHITECTURE.md §13.10). It refines the element monoid every range structure
// shares (Domain/Mathematics/Algebra's IMonoid) with an action only this structure has, so it stays
// structure-local (§11.4).
/// <summary>
/// The algebra a segment tree with range updates runs on: an element monoid plus updates it can
/// defer, compose and apply.
/// </summary>
/// <typeparam name="Element">The element type the monoid combines.</typeparam>
/// <typeparam name="TUpdate">The type of one range update.</typeparam>
public interface IRangeUpdateOperation<Element, TUpdate>
    : IMonoid<Element>, IRangeAction<Element, TUpdate>
{
}
