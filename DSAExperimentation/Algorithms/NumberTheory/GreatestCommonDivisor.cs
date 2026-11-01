using System.Numerics;

namespace DSAExperimentation.Algorithms.NumberTheory;

// Euclid's algorithm, iterative, over any binary integer. Generic math rather than one copy per
// width: each value type closes the method separately at JIT time, so int and long each get their
// own specialized code with the operators inlined - the same cost as a hand-written int version.
//
// The answer is never negative: both inputs are taken by magnitude, which is what every caller that
// reduces a slope or a fraction already relied on. gcd(0, 0) is 0 and gcd(0, x) is |x|, so 0 is the
// identity a running gcd starts from. Precondition law, unchecked beyond what Integer.Abs itself
// checks: Integer.MinValue has no magnitude in Integer, and Integer.Abs throws on it as Math.Abs does.
//
// Named for what it computes rather than "Gcd": dozens of solution files already declare a private
// method called Gcd, and inside such a class a type named Gcd would resolve to the method.
/// <summary>Euclid's greatest common divisor over any binary integer type.</summary>
public static class GreatestCommonDivisor
{
    /// <summary>Finds two integers' greatest common divisor by Euclid's algorithm.</summary>
    /// <param name="first">One integer, of either sign.</param>
    /// <param name="second">The other integer, of either sign.</param>
    /// <returns>The divisor, never negative: the other's magnitude when one is 0, and 0 when both are.</returns>
    /// <exception cref="OverflowException">Thrown when an argument is a signed type's MinValue.</exception>
    public static Integer Of<Integer>(Integer first, Integer second)
        where Integer : IBinaryInteger<Integer>
    {
        first = Integer.Abs(first);
        second = Integer.Abs(second);

        while (second != Integer.Zero)
        {
            (first, second) = (second, first % second);
        }

        return first;
    }
}
