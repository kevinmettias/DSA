using DSAExperimentation.Domain.Mathematics.Modular;

namespace DSAExperimentation.Algorithms.NumberTheory;

// Factorials and their inverses modulo a prime, prepared once up to a bound so every binomial
// coefficient afterward is two multiplications. The table is the algorithm; which prime it reports
// in is the caller's, chosen by TModulus. A struct witness whose modulus is a compile-time constant
// folds into the code, so each reduction is a multiplication by the constant's reciprocal rather than
// a division.
//
// Build fills the factorials upward, inverts only the last with one Fermat inverse, and fills the
// inverses downward, since 1/(i)! = (i + 1) * 1/(i + 1)!. The two halves are exposed beside Choose
// because most callers want a product: a multinomial coefficient is one factorial times one inverse
// factorial per bucket.
/// <summary>Factorials, their inverses and binomial coefficients modulo a prime, prepared up to a fixed n.</summary>
/// <typeparam name="TModulus">The prime modulus the table reports in.</typeparam>
/// <param name="factorial">The factorials 0! through n!, each modulo the prime.</param>
/// <param name="inverseFactorial">The modular inverses of those factorials, in the same order.</param>
public sealed class FactorialTable<TModulus>(long[] factorial, long[] inverseFactorial)
    where TModulus : struct, IPrimeModulus
{
    /// <summary>Prepares the tables for every argument from 0 through <paramref name="maxN"/>.</summary>
    /// <param name="maxN">The largest argument the table answers for; at least 0.</param>
    /// <returns>The prepared table.</returns>
    public static FactorialTable<TModulus> Build(int maxN)
    {
        var factorial = new long[maxN + 1];
        var inverseFactorial = new long[maxN + 1];
        factorial[0] = 1;

        for (var i = 1; i <= maxN; i++)
        {
            factorial[i] = factorial[i - 1] * i % TModulus.Value;
        }

        inverseFactorial[maxN] = ModularInverse.ByFermat<TModulus>(factorial[maxN]);

        for (var i = maxN - 1; i >= 0; i--)
        {
            inverseFactorial[i] = inverseFactorial[i + 1] * (i + 1) % TModulus.Value;
        }

        return new FactorialTable<TModulus>(factorial, inverseFactorial);
    }

    /// <summary>Reads one factorial from the table.</summary>
    /// <param name="argument">The number to take the factorial of, from 0 through the table's maxN.</param>
    /// <returns>The argument's factorial, modulo the prime.</returns>
    public long Factorial(int argument) => factorial[argument];

    /// <summary>Reads the modular inverse of one factorial from the table.</summary>
    /// <param name="argument">The number whose factorial is inverted, from 0 through the table's maxN.</param>
    /// <returns>The inverse of the argument's factorial, modulo the prime.</returns>
    public long InverseFactorial(int argument) => inverseFactorial[argument];

    /// <summary>Counts the ways to choose some items from a set, modulo the prime.</summary>
    /// <param name="totalCount">How many items there are, at most the table's maxN.</param>
    /// <param name="chosenCount">How many of them are chosen.</param>
    /// <returns>
    /// The binomial coefficient modulo the prime, or 0 when either count is negative or more are chosen
    /// than exist.
    /// </returns>
    public long Choose(int totalCount, int chosenCount)
    {
        if (IsImpossibleChoice(totalCount, chosenCount))
        {
            return 0;
        }

        var withChosen = factorial[totalCount] * inverseFactorial[chosenCount] % TModulus.Value;

        return withChosen * inverseFactorial[totalCount - chosenCount] % TModulus.Value;
    }

    // nCr counts subsets of `chosenCount` taken from `totalCount`, so a negative total or chosen count,
    // or a chosen count past the total, names a subset that cannot exist.
    private static bool IsImpossibleChoice(int totalCount, int chosenCount)
        => totalCount < 0 || chosenCount < 0 || chosenCount > totalCount;
}
