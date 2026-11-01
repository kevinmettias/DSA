using System.Numerics;

namespace DSAExperimentation.Algorithms.NumberTheory;

// lcm(a, b) = |a| / gcd(a, b) * |b|, dividing before multiplying so the intermediate never exceeds
// the answer. lcm(0, x) is 0. Never negative, for the same reason GreatestCommonDivisor is not.
//
// Precondition law: the answer must fit in Integer. The multiply is unchecked, so an lcm past
// Integer.MaxValue wraps silently rather than throwing - a caller whose inputs can produce one widens
// them first (lcm of two ints as long), which is what the solutions that need large lcms already do.
/// <summary>The least common multiple over any binary integer type.</summary>
public static class LeastCommonMultiple
{
    /// <summary>
    /// Finds two integers' least common multiple, which wraps silently if it does not fit in their type.
    /// </summary>
    /// <param name="first">One integer, of either sign.</param>
    /// <param name="second">The other integer, of either sign.</param>
    /// <returns>The multiple, never negative when it fits, and 0 when either integer is 0.</returns>
    /// <exception cref="OverflowException">
    /// Thrown when neither argument is 0 and one is a signed type's MinValue.
    /// </exception>
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
