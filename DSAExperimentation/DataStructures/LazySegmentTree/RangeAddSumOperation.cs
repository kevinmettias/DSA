using System.Numerics;

namespace DSAExperimentation.DataStructures.LazySegmentTree;

// Range-add, range-sum: a pending add of d across k elements changes their sum by d*k
// (ApplyUpdate), and two pending adds on the same node simply accumulate (ComposeUpdate).
internal readonly struct RangeAddSumOperation<Element> : IRangeUpdateOperation<Element, Element>
    where Element : INumber<Element>
{
    public static Element Identity => Element.Zero;

    public static Element NoUpdate => Element.Zero;

    public static Element Combine(Element left, Element right) => left + right;

    public static Element ComposeUpdate(Element outer, Element inner) => outer + inner;

    public static Element ApplyUpdate(Element aggregate, Element update, int rangeLength) => aggregate + (update * Element.CreateChecked(rangeLength));
}
