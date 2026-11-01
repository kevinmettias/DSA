using System.Numerics;

namespace DSAExperimentation.DataStructures.LazySegmentTree;

// Range-add, range-sum: a pending add of d across k elements changes their sum by d*k
// (ApplyUpdate), and two pending adds on the same node simply accumulate (ComposeUpdate).
/// <summary>
/// The range-add, range-sum algebra for LazySegmentTree: an update adds its value to every element of a range.
/// </summary>
public readonly struct RangeAddSumOperation<Element> : IRangeUpdateOperation<Element, Element>
    where Element : INumber<Element>
{
    /// <summary>Zero, the sum of an empty range.</summary>
    public static Element Identity => Element.Zero;

    public static Element NoUpdate => Element.Zero;

    /// <summary>Adds two ranges' sums into the sum of both.</summary>
    /// <param name="left">The sum of the left range.</param>
    /// <param name="right">The sum of the right range.</param>
    /// <returns><paramref name="left"/> plus <paramref name="right"/>.</returns>
    public static Element Combine(Element left, Element right) => left + right;

    public static Element ComposeUpdate(Element outer, Element inner) => outer + inner;

    public static Element ApplyUpdate(Element aggregate, Element update, int rangeLength) => aggregate + (update * Element.CreateChecked(rangeLength));
}
