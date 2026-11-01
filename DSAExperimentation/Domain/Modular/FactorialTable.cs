namespace DSAExperimentation.Domain.Modular;

// A prepared nCr(n, r) mod 1e9+7 lookup: Build(maxN) fills factorial and inverse-
// factorial tables once, O(maxN) with a single ModularArithmetic.Inverse call, so
// every Choose afterward is O(1). It sits beside ModularArithmetic, at this tier's
// root, for the same reason ModularArithmetic does: what fixes its content is the
// modulus, a LeetCode reporting convention rather than a property of any
// algorithm, so it is LeetCode code rather than an Algorithms primitive (17.6).
//
// This type was first written beside CountValidSequencesSolution, whose doc comment
// said it should move to a shared home once other problems needed the same table -
// a witness generic enough to be a real "nCr table" primitive "would still need
// callers elsewhere in the catalogue before that move is honest, per 17.6's 'sharing
// is not the classification test'". That condition is now met: eight counting
// solutions across the catalogue were each building this same factorial/inverse-
// factorial pair inline.
/// <summary>Factorials, their inverses and binomial coefficients modulo 1e9+7, prepared up to a fixed n.</summary>
/// <param name="factorial">The factorials 0! through n!, each modulo 1e9+7.</param>
/// <param name="inverseFactorial">The modular inverses of those factorials, in the same order.</param>
public sealed class FactorialTable(long[] factorial, long[] inverseFactorial)
{
    /// <summary>Prepares the tables for every argument from 0 through <paramref name="maxN"/>.</summary>
    /// <param name="maxN">The largest argument the table answers for; at least 0.</param>
    /// <returns>The prepared table.</returns>
    public static FactorialTable Build(int maxN)
    {
        var factorial = new long[maxN + 1];
        var inverseFactorial = new long[maxN + 1];
        factorial[0] = 1;

        for (var i = 1; i <= maxN; i++)
        {
            factorial[i] = factorial[i - 1] * i % ModularArithmetic.Modulo;
        }

        inverseFactorial[maxN] = ModularArithmetic.Inverse(factorial[maxN]);

        for (var i = maxN - 1; i >= 0; i--)
        {
            inverseFactorial[i] = inverseFactorial[i + 1] * (i + 1) % ModularArithmetic.Modulo;
        }

        return new FactorialTable(factorial, inverseFactorial);
    }

    // The two halves are exposed separately as well as through Choose because most
    // callers want a product, not one coefficient: a multinomial coefficient is
    // factorial[n] times one inverse factorial per bucket, and the infection and
    // balanced-permutation counts read an inverse factorial on its own.
    /// <summary>Reads one factorial from the table.</summary>
    /// <param name="argument">The number to take the factorial of, from 0 through the table's maxN.</param>
    /// <returns>The argument's factorial modulo 1e9+7.</returns>
    public long Factorial(int argument) => factorial[argument];

    /// <summary>Reads the modular inverse of one factorial from the table.</summary>
    /// <param name="argument">The number whose factorial is inverted, from 0 through the table's maxN.</param>
    /// <returns>The inverse of the argument's factorial modulo 1e9+7.</returns>
    public long InverseFactorial(int argument) => inverseFactorial[argument];

    /// <summary>Counts the ways to choose some items from a set, modulo 1e9+7.</summary>
    /// <param name="totalCount">How many items there are, at most the table's maxN.</param>
    /// <param name="chosenCount">How many of them are chosen.</param>
    /// <returns>
    /// The binomial coefficient modulo 1e9+7, or 0 when either count is negative or more are chosen than
    /// exist.
    /// </returns>
    public long Choose(int totalCount, int chosenCount) =>
        IsImpossibleChoice(totalCount, chosenCount)
            ? 0
            : factorial[totalCount] * inverseFactorial[chosenCount] % ModularArithmetic.Modulo
                * inverseFactorial[totalCount - chosenCount] % ModularArithmetic.Modulo;

    // nCr counts subsets of `chosenCount` taken from `totalCount`, so a negative total or
    // chosen count, or a chosen count past the total, names a subset that cannot exist.
    private static bool IsImpossibleChoice(int totalCount, int chosenCount) =>
        totalCount < 0 || chosenCount < 0 || chosenCount > totalCount;
}
