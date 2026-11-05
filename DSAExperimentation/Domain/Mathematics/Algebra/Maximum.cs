using System.Numerics;

namespace DSAExperimentation.Domain.Mathematics.Algebra;

// Identity is Element.MinValue - the mirror image of Minimum<Element>'s reasoning: Combine(MinValue, x)
// must always resolve to x for a "no overlap" query branch to be invisible to the result.
/// <summary>
/// Combines elements by keeping the larger, or the left one on a tie, with <c>Element.MinValue</c> as the identity.
/// </summary>
public readonly struct Maximum<Element> : IMonoid<Element>
    where Element : IComparable<Element>, IMinMaxValue<Element>
{
    public static Element Identity => Element.MinValue;

    public static Element Combine(Element left, Element right)
    {
        var leftIsAtLeastAsLarge = left.CompareTo(right) >= 0;
        return leftIsAtLeastAsLarge ? left : right;
    }
}
