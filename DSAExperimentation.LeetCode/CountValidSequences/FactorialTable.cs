using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountValidSequences;

// A prepared nCr(n, r) mod 1e9+7 lookup: Build(maxN) fills factorial and inverse-
// factorial tables once, O(maxN) with a single ModularArithmetic.Inverse call, so
// every Choose afterward is O(1). Answers this one problem's queries and nothing
// else - CountValidSequencesSolution needs at most two Choose calls per (n, k),
// which is exactly what earns it a place beside the solution rather than in
// Domain/Modular (17.3): a witness generic enough to be a real "nCr table"
// primitive would still need callers elsewhere in the catalogue before that move
// is honest, per 17.6's "sharing is not the classification test".
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

    public long Choose(int n, int r)
    {
        if (IsImpossibleChoice(n, r))
        {
            return 0;
        }

        return factorial[n] * inverseFactorial[r] % ModularArithmetic.Modulo * inverseFactorial[n - r] % ModularArithmetic.Modulo;
    }

    // nCr counts subsets of r taken from n, so a negative n or r, or an r past n,
    // names a subset that cannot exist.
    private static bool IsImpossibleChoice(int n, int r) => n < 0 || r < 0 || r > n;
}
