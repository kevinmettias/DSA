using System.Numerics;

namespace DSAExperimentation.DataStructures.FenwickTree;

internal readonly struct SumOperation<Element> : IGroupOperation<Element>
    where Element : INumber<Element>
{
    public static Element Identity => Element.Zero;

    public static Element Combine(Element left, Element right) => left + right;

    public static Element Invert(Element value) => -value;
}
