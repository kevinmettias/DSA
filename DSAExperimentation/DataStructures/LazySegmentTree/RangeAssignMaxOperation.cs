using System.Numerics;
using DSAExperimentation.DataStructures.ElementAlgebra;

namespace DSAExperimentation.DataStructures.LazySegmentTree;

// Range-assign, range-max: TUpdate is Element? rather than Element, because "no pending update" must be
// distinguishable from "assign to this node's own minimum representable value" - a real value a
// caller can legitimately assign. NoUpdate is null; a newer assignment always overwrites whatever
// was already pending (ComposeUpdate keeps outer, discards inner - assignment has no memory of
// what it replaces), and assigning v to any number of elements makes their max v regardless of
// rangeLength. The aggregate itself is plain max, so Identity and Combine are MaxOperation's.
internal readonly struct RangeAssignMaxOperation<Element> : IRangeUpdateOperation<Element, Element?>
    where Element : struct, IComparable<Element>, IMinMaxValue<Element>
{
    public static Element Identity => MaxOperation<Element>.Identity;

    public static Element? NoUpdate => null;

    public static Element Combine(Element left, Element right) => MaxOperation<Element>.Combine(left, right);

    public static Element? ComposeUpdate(Element? outer, Element? inner) => outer;

    public static Element ApplyUpdate(Element aggregate, Element? update, int rangeLength) => update ?? aggregate;
}
