namespace DSAExperimentation.LeetCode.SimplifiedFractions;

// LeetCode 1447. Simplified Fractions: list every fraction strictly between 0 and 1
// with denominator at most n, in simplest form.
//
// numerator/denominator is already simplest iff gcd(numerator, denominator) == 1,
// so walking every denominator 2..n and every numerator 1..denominator-1 emits each
// simplified fraction exactly once, with no duplicate to filter out. No repo
// container or algorithm primitive applies - there is nothing to compose over two
// running integers and a result list, the same "nothing to compose over plain
// integers" case PowXnSolution and CheckIfItIsAGoodArraySolution already are - so
// the one real algorithmic choice the problem has is how each pairwise gcd is
// computed, and that is what the two strategies here differ in.
internal static class SimplifiedFractionsSolution
{
    // Every fraction is proper, so the smallest denominator that admits one is 2.
    private const int MinDenominator = 2;

    // gcd == 1 is exactly "already in simplest form".
    private const int Coprime = 1;

    // Both gcds are stateless, so one instance each serves every call and the benchmark
    // arms that measure the two listings allocate nothing to pick one.
    private static readonly IGcdStrategy TrialDivisionGcd = new GcdByTrialDivision();
    private static readonly IGcdStrategy EuclideanGcd = new GcdByEuclideanAlgorithm();

    // The textbook answer: find each gcd by counting divisors down from
    // min(a, b) until one divides both - O(min(a, b)) per pair. Deliberately
    // written without this repo's primitives; it is the arm the strategy below has
    // to justify itself against.
    public static List<string> ListFractionsByTrialDivisionGcd(int n) =>
        ListFractions(n, TrialDivisionGcd);

    // The standard Euclidean algorithm, O(log min(a, b)) per pair - the same
    // private-helper shape CheckIfItIsAGoodArraySolution, NthMagicalNumberSolution
    // and XOfAKindInADeckOfCardsSolution already reuse inline.
    public static List<string> ListFractionsByEuclideanGcd(int n) =>
        ListFractions(n, EuclideanGcd);

    // The one question the two arms answer differently: the greatest common divisor of
    // the fraction's two terms, which is exactly what decides whether the fraction is
    // already in simplest form. Both of its inputs are named here, and this is where the
    // contract the bare form had nowhere to write goes - the result is always at least
    // Coprime, so a caller comparing it against Coprime never has to special-case a zero.
    private interface IGcdStrategy
    {
        int Compute(int a, int b);
    }

    // The trial-division arm: walk divisors down from min(a, b) until one divides both.
    private sealed class GcdByTrialDivision : IGcdStrategy
    {
        public int Compute(int a, int b)
        {
            for (var divisor = Math.Min(a, b); divisor >= Coprime; divisor--)
            {
                if (a % divisor == 0 && b % divisor == 0)
                {
                    return divisor;
                }
            }

            return Coprime;
        }
    }

    // The enumeration itself, shared by both strategies so that the only thing they
    // differ in is the gcd they are handed.
    private static List<string> ListFractions(int n, IGcdStrategy gcd)
    {
        var fractions = new List<string>();

        for (var denominator = MinDenominator; denominator <= n; denominator++)
        {
            for (var numerator = 1; numerator < denominator; numerator++)
            {
                if (gcd.Compute(numerator, denominator) == Coprime)
                {
                    fractions.Add($"{numerator}/{denominator}");
                }
            }
        }

        return fractions;
    }

    private sealed class GcdByEuclideanAlgorithm : IGcdStrategy
    {
        public int Compute(int a, int b) => b == 0 ? a : Compute(b, a % b);
    }
}
