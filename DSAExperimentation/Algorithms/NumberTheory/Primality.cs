using System.Numerics;

namespace DSAExperimentation.Algorithms.NumberTheory;

// Primality of one value by trial division: 2 alone, then odd divisors up to the square root,
// O(sqrt(value)). For many values under one bound, PrimeSieve answers all of them at once instead.
//
// The loop bound is divisor <= value / divisor rather than divisor * divisor <= value, so it cannot
// overflow Integer for a value near Integer.MaxValue.
/// <summary>Primality of one value by trial division, in O(sqrt(value)) time.</summary>
public static class Primality
{
    /// <summary>Decides whether a value is prime by trial division.</summary>
    /// <param name="value">The value to test, of any binary integer type.</param>
    /// <returns>True when the value is prime; false for every value below 2.</returns>
    public static bool IsPrime<Integer>(Integer value)
        where Integer : IBinaryInteger<Integer>
    {
        var two = Integer.One + Integer.One;

        if (value < two)
        {
            return false;
        }

        if (Integer.IsEvenInteger(value))
        {
            return value == two;
        }

        for (var divisor = two + Integer.One; divisor <= value / divisor; divisor += two)
        {
            if (value % divisor == Integer.Zero)
            {
                return false;
            }
        }

        return true;
    }
}
