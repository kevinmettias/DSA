namespace DSAExperimentation.Domain.Modular;

// A prepared nCr(n, r) mod 1e9+7 lookup: Build(maxN) fills factorial and inverse-
// factorial tables once, O(maxN) with a single ModularArithmetic.Inverse call, so
// every Choose afterward is O(1). It sits in Domain/Modular for the same reason
// ModularArithmetic does: what fixes its content is the modulus, a LeetCode
// reporting convention rather than a property of any algorithm, so it is domain
// code rather than an Algorithms primitive (17.6).
//
// This type was first written beside CountValidSequencesSolution, whose doc comment
// said it should move here once other problems needed the same table - a witness
// generic enough to be a real "nCr table" primitive "would still need callers
// elsewhere in the catalogue before that move is honest, per 17.6's 'sharing is not
// the classification test'". That condition is now met: eight counting solutions
// across the catalogue were each building this same factorial/inverse-factorial
// pair inline.
internal sealed class FactorialTable(long[] factorial, long[] inverseFactorial)
{
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
    public long Factorial(int n) => factorial[n];

    public long InverseFactorial(int n) => inverseFactorial[n];

    public long Choose(int n, int r) =>
        IsImpossibleChoice(n, r)
            ? 0
            : factorial[n] * inverseFactorial[r] % ModularArithmetic.Modulo
                * inverseFactorial[n - r] % ModularArithmetic.Modulo;

    // nCr counts subsets of r taken from n, so a negative n or r, or an r past n,
    // names a subset that cannot exist.
    private static bool IsImpossibleChoice(int n, int r) => n < 0 || r < 0 || r > n;
}
