using System.Numerics;
using DSAExperimentation.DataStructures.ElementAlgebra;

namespace DSAExperimentation.Algorithms.NumberTheory;

// The gcd as an element algebra: associative and commutative, with 0 as its identity because
// gcd(0, x) = x - so a SegmentTree over it answers range-gcd queries the way one over SumOperation
// answers range sums, and an empty range is the invisible 0. It belongs to the ICombineOperation
// family but composes an Algorithms primitive, GreatestCommonDivisor, so it sits here beside that
// primitive rather than in DataStructures/ElementAlgebra, which no Algorithms type may be imported
// into. Never negative, for GreatestCommonDivisor's reason.
internal readonly struct GcdOperation<Element> : ICombineOperation<Element>
    where Element : IBinaryInteger<Element>
{
    public static Element Identity => Element.Zero;

    public static Element Combine(Element left, Element right) => GreatestCommonDivisor.Of(left, right);
}
