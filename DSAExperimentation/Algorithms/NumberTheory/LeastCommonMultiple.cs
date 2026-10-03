using System.Numerics;

namespace DSAExperimentation.Algorithms.NumberTheory;

// lcm(a, b) = |a| / gcd(a, b) * |b|, dividing before multiplying so the intermediate never exceeds
// the answer. lcm(0, x) is 0. Never negative, for the same reason GreatestCommonDivisor is not.
//
// Precondition law: the answer must fit in Integer. The multiply is unchecked, so an lcm past
// Integer.MaxValue wraps silently rather than throwing - a caller whose inputs can produce one widens
// them first (lcm of two ints as long), which is what the solutions that need large lcms already do.
internal static class LeastCommonMultiple
{
    public static Integer Of<Integer>(Integer first, Integer second)
        where Integer : IBinaryInteger<Integer>
    {
        if (first == Integer.Zero || second == Integer.Zero)
        {
            return Integer.Zero;
        }

        return Integer.Abs(first) / GreatestCommonDivisor.Of(first, second) * Integer.Abs(second);
    }
}
