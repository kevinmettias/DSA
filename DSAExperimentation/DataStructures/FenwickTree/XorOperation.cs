using System.Numerics;

namespace DSAExperimentation.DataStructures.FenwickTree;

// XOR is self-inverse - a XOR b XOR b == a - so Invert is the identity function, not negation.
internal readonly struct XorOperation<Element> : IGroupOperation<Element>
    where Element : IBinaryInteger<Element>
{
    public static Element Identity => Element.Zero;

    public static Element Combine(Element left, Element right) => left ^ right;

    public static Element Invert(Element value) => value;
}
