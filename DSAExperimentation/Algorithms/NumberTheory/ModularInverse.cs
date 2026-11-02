using DSAExperimentation.Domain.Mathematics.Modular;

namespace DSAExperimentation.Algorithms.NumberTheory;

// The multiplicative inverse modulo a prime, by Fermat's little theorem (Domain/Mathematics/Modular's
// IPrimeModulus states it): a^(p - 2), one modular exponentiation. The extended Euclidean algorithm
// is the other way to the same number, and the one a composite modulus needs; nothing here asks for
// it yet.
/// <summary>Multiplicative inverses modulo a prime.</summary>
public static class ModularInverse
{
    // Fermat: a^(p - 1) is 1 modulo p, so a^(p - 2) is a's inverse.
    private const long FermatExponentOffset = 2;

    /// <summary>Finds a value's multiplicative inverse modulo a prime, as its (p - 2)th power.</summary>
    /// <typeparam name="TModulus">The prime modulus.</typeparam>
    /// <param name="value">The value to invert; it must not be a multiple of the modulus.</param>
    /// <returns>The residue that multiplies with the value to 1, modulo the prime.</returns>
    public static long ByFermat<TModulus>(long value)
        where TModulus : struct, IPrimeModulus
        => ModularPower.Of(value, TModulus.Value - FermatExponentOffset, TModulus.Value);
}
