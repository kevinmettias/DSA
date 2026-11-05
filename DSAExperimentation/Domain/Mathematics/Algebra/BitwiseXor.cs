using System.Numerics;

namespace DSAExperimentation.Domain.Mathematics.Algebra;

// XOR is self-inverse - a XOR b XOR b == a - so Invert is the identity function, not negation.
/// <summary>Bitwise XOR as a group operation, with identity zero and every value its own inverse.</summary>
public readonly struct BitwiseXor<Element> : IAbelianGroup<Element>
    where Element : IBinaryInteger<Element>
{
    /// <summary>Zero, since XOR with zero leaves any value unchanged.</summary>
    public static Element Identity => Element.Zero;

    /// <summary>Combines two values by bitwise XOR.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns>The bitwise XOR of <paramref name="left"/> and <paramref name="right"/>.</returns>
    public static Element Combine(Element left, Element right) => left ^ right;

    public static Element Invert(Element value) => value;
}
