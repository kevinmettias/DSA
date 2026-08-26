using System.Numerics;

namespace DSAExperimentation.DataStructures.RangeFenwickTree;

internal readonly struct ScaledSumOperation<Element> : IScaledGroupOperation<Element>
    where Element : INumber<Element>
{
    public static Element Identity => Element.Zero;

    public static Element Combine(Element left, Element right) => left + right;

    public static Element Invert(Element value) => -value;

    public static Element Scale(Element value, long count) => value * Element.CreateChecked(count);
}
