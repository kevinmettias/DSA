using System.Numerics;

namespace DSAExperimentation.DataStructures.SegmentTree;

// Identity is Element.MinValue - the mirror image of MinOperation<Element>'s reasoning: Combine(MinValue, x)
// must always resolve to x for a "no overlap" query branch to be invisible to the result.
internal readonly struct MaxOperation<Element> : ICombineOperation<Element>
    where Element : IComparable<Element>, IMinMaxValue<Element>
{
    public static Element Identity => Element.MinValue;

    public static Element Combine(Element left, Element right)
    {
        var leftIsAtLeastAsLarge = left.CompareTo(right) >= 0;
        return leftIsAtLeastAsLarge ? left : right;
    }
}
