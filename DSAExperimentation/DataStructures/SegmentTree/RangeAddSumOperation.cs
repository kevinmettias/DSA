using System.Numerics;

namespace DSAExperimentation.DataStructures.SegmentTree;

// Range-add, range-sum: a pending add of d across k elements changes their sum by d*k
// (ApplyUpdate), and two pending adds on the same node simply accumulate (ComposeUpdate).
/// <summary>
/// The range-add, range-sum algebra for a segment tree: an update adds its value to every element
/// of a range.
/// </summary>
public readonly struct RangeAddSumOperation<Element> : IRangeUpdateOperation<Element, Element>
    where Element : INumber<Element>
{
    /// <summary>Zero, the sum of an empty range.</summary>
    public static Element Identity => Element.Zero;

    /// <summary>Zero, the add that changes nothing.</summary>
    public static Element NoUpdate => Element.Zero;

    /// <summary>Adds two ranges' sums into the sum of both.</summary>
    /// <param name="left">The sum of the left range.</param>
    /// <param name="right">The sum of the right range.</param>
    /// <returns><paramref name="left"/> plus <paramref name="right"/>.</returns>
    public static Element Combine(Element left, Element right) => left + right;

    /// <summary>Adds two pending adds into one.</summary>
    /// <param name="outer">The add arriving now.</param>
    /// <param name="inner">The add already pending.</param>
    /// <returns><paramref name="outer"/> plus <paramref name="inner"/>.</returns>
    public static Element ComposeUpdate(Element outer, Element inner) => outer + inner;

    /// <summary>Adds an add to every element of a range to the range's sum.</summary>
    /// <param name="aggregate">The range's sum before the add.</param>
    /// <param name="update">The value added to each element.</param>
    /// <param name="rangeLength">How many elements the range covers.</param>
    /// <returns>The sum after <paramref name="update"/> is added to each element.</returns>
    public static Element ApplyUpdate(Element aggregate, Element update, int rangeLength) => aggregate + (update * Element.CreateChecked(rangeLength));
}
