using System.Numerics;

namespace DSAExperimentation.DataStructures.ElementAlgebra;

// Addition, at the deepest position of the chain it can reach: a monoid for SegmentTree, a group for
// FenwickTree (Invert is negation) and a scaled group for RangeFenwickTree (Scale is multiplication
// by the count, which meets that contract's O(1) law). One witness therefore serves all three
// summing structures, where each used to carry its own identical copy.
internal readonly struct SumOperation<Element> : IScaledGroupOperation<Element>
    where Element : INumber<Element>
{
    public static Element Identity => Element.Zero;

    public static Element Combine(Element left, Element right) => left + right;

    public static Element Invert(Element value) => -value;

    public static Element Scale(Element value, long count) => value * Element.CreateChecked(count);
}
