using System.Numerics;

namespace DSAExperimentation.DataStructures.SegmentTree;

internal readonly struct SumOperation<Element> : ICombineOperation<Element>
    where Element : INumber<Element>
{
    public static Element Identity => Element.Zero;

    public static Element Combine(Element left, Element right) => left + right;
}
