using System.Numerics;
using DSAExperimentation.DataStructures.ElementAlgebra;

namespace DSAExperimentation.DataStructures.LazySegmentTree;

// Range-assign, range-max: TUpdate is Element? rather than Element, because "no pending update" must be
// distinguishable from "assign to this node's own minimum representable value" - a real value a
// caller can legitimately assign. NoUpdate is null; a newer assignment always overwrites whatever
// was already pending (ComposeUpdate keeps outer, discards inner - assignment has no memory of
// what it replaces), and assigning v to any number of elements makes their max v regardless of
// rangeLength. The aggregate itself is plain max, so Identity and Combine are MaxOperation's.
/// <summary>
/// The range-assign, range-max algebra for LazySegmentTree: an update is a value to assign, with null meaning none.
/// </summary>
public readonly struct RangeAssignMaxOperation<Element> : IRangeUpdateOperation<Element, Element?>
    where Element : struct, IComparable<Element>, IMinMaxValue<Element>
{
    /// <summary><c>Element.MinValue</c>, the maximum of an empty range.</summary>
    public static Element Identity => MaxOperation<Element>.Identity;

    public static Element? NoUpdate => null;

    /// <summary>Takes the larger of two ranges' maxima.</summary>
    /// <param name="left">The maximum of the left range.</param>
    /// <param name="right">The maximum of the right range.</param>
    /// <returns>The larger of <paramref name="left"/> and <paramref name="right"/>.</returns>
    public static Element Combine(Element left, Element right) => MaxOperation<Element>.Combine(left, right);

    public static Element? ComposeUpdate(Element? outer, Element? inner) => outer;

    public static Element ApplyUpdate(Element aggregate, Element? update, int rangeLength) => update ?? aggregate;
}
