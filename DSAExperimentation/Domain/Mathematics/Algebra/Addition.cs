using System.Numerics;

namespace DSAExperimentation.Domain.Mathematics.Algebra;

// Addition, at the deepest position of the chain it can reach: a monoid for SegmentTree, an abelian
// group for FenwickTree (Invert is negation) and an integer module for RangeFenwickTree (Scale is
// multiplication by the count, which meets that contract's O(1) law). One witness therefore serves all three
// summing structures, where each used to carry its own identical copy.
/// <summary>
/// Addition as an element algebra: zero is the identity, negation the inverse, and multiplication by the
/// count the scale.
/// </summary>
public readonly struct Addition<Element> : IIntegerModule<Element>
    where Element : INumber<Element>
{
    /// <summary>Zero, the identity of addition.</summary>
    public static Element Identity => Element.Zero;

    /// <summary>Adds two elements.</summary>
    /// <param name="left">The first addend.</param>
    /// <param name="right">The second addend.</param>
    /// <returns>The sum <c>left + right</c>.</returns>
    public static Element Combine(Element left, Element right) => left + right;

    /// <summary>Negates an element.</summary>
    /// <param name="value">The element to negate.</param>
    /// <returns>The negation <c>-value</c>.</returns>
    public static Element Invert(Element value) => -value;

    public static Element Scale(Element value, long count) => value * Element.CreateChecked(count);
}
