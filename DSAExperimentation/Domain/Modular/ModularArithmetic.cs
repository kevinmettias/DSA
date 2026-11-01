using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.Domain.Modular;

// Arithmetic modulo 1e9+7 - LeetCode's own convention for "report the answer mod a
// large prime", not a property of any algorithm, which is why this is domain code
// rather than an Algorithms primitive. Every counting problem in the catalogue that
// asks for a result "modulo 10^9 + 7" shares this constant and these two
// operations. The exponentiation itself is the general algorithm in
// Algorithms/NumberTheory/ModularPower; what this type adds is the modulus.
/// <summary>Arithmetic modulo 1e9+7, the prime LeetCode asks counting answers to be reported in.</summary>
public static class ModularArithmetic
{
    public const long Modulo = 1_000_000_007;

    // Fermat's little theorem: for prime p and a not divisible by p,
    // a^(p-2) is a's multiplicative inverse mod p.
    private const long InverseExponentOffset = 2;

    /// <summary>Finds a value's multiplicative inverse modulo 1e9+7.</summary>
    /// <param name="value">The value to invert; it must not be a multiple of the modulus.</param>
    /// <returns>The number that multiplies with the value to 1, modulo 1e9+7.</returns>
    public static long Inverse(long value) => Power(value, Modulo - InverseExponentOffset);

    /// <summary>Raises a value to a power modulo 1e9+7.</summary>
    /// <param name="value">The base.</param>
    /// <param name="exponent">The power, at least 0.</param>
    /// <returns>The base raised to the exponent, modulo 1e9+7.</returns>
    public static long Power(long value, long exponent) => ModularPower.Of(value, exponent, Modulo);
}
