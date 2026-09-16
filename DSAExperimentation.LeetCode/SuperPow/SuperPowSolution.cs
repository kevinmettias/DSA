namespace DSAExperimentation.LeetCode.SuperPow;

// LeetCode 372. Super Pow: a^b mod 1337 where b is an arbitrarily long exponent given
// digit-by-digit (up to 2000 digits - far past any fixed-width integer). No repo
// container or algorithm primitive applies here - there is nothing to compose over a
// handful of running scalars (result/base/exponent), the same "lighter repo-primitive
// fit" case this repo already accepted for Pow(x, n) and Power of Two.
internal static class SuperPowSolution
{
    private const int Modulus = 1337;
    private const int HornerDigitBase = 10;

    // The textbook baseline: reconstruct the exponent from its digits (Horner's rule),
    // then multiply by baseValue once per unit of exponent - O(exponent). Deliberately
    // BCL-only, and only survives at benchmark-sized exponents; it is the arm
    // SuperPowByHornerSquaring has to justify itself against, exactly because
    // LeetCode's real 2000-digit exponents make this loop impossible to finish.
    public static int SuperPowByRepeatedMultiplication(int baseValue, int[] exponentDigits)
    {
        var exponent = 0L;

        foreach (var digit in exponentDigits)
        {
            exponent = exponent * HornerDigitBase + digit;
        }

        var result = 1L;
        var baseTerm = baseValue % Modulus;

        for (var i = 0L; i < exponent; i++)
        {
            result = result * baseTerm % Modulus;
        }

        return (int)result;
    }

    // Horner's rule over the digits combined with modular exponentiation by squaring
    // (PowXn's squaring loop, folded under Modulus at every multiplication) keeps every
    // intermediate value inside [0, Modulus) and does O(digits * log 10) work total,
    // handling exponents far past any fixed-width integer.
    public static int SuperPowByHornerSquaring(int baseValue, int[] exponentDigits)
    {
        var result = 1L;
        var baseTerm = baseValue % Modulus;

        foreach (var digit in exponentDigits)
        {
            result = ModPow(result, HornerDigitBase) * ModPow(baseTerm, digit) % Modulus;
        }

        return (int)result;
    }

    private static long ModPow(long value, int exponent)
    {
        var result = 1L;
        value %= Modulus;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result = result * value % Modulus;
            }

            value = value * value % Modulus;
            exponent >>= 1;
        }

        return result;
    }
}
