using System.Numerics;

namespace DSAExperimentation.DataStructures.SegmentTree;

// Identity is Element.MaxValue, not a caller-supplied sentinel: Combine(MaxValue, x) must always
// resolve to x for a "no overlap" query branch to be invisible to the result, the same role
// positive infinity plays for min in the untyped math.
internal readonly struct MinOperation<Element> : ICombineOperation<Element>
    where Element : IComparable<Element>, IMinMaxValue<Element>
{
    public static Element Identity => Element.MaxValue;

    public static Element Combine(Element left, Element right)
    {
        var leftIsNoLargerThanRight = left.CompareTo(right) <= 0;
        return leftIsNoLargerThanRight ? left : right;
    }
}
