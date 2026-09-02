using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfStringsWhichCanBeRearrangedToContainSubstring;

// LeetCode 2930. Number of Strings Which Can Be Rearranged to Contain Substring
// "leet": count length-n lowercase strings (mod 1e9+7) that have SOME permutation
// containing "leet". Rearrangement is free, so this is purely a question about
// letter counts: a string qualifies exactly when it has at least one 'l', at
// least two 'e's and at least one 't' - any such multiset can always be arranged
// as "leet" followed by the rest in any order.
internal static class NumberOfStringsWhichCanBeRearrangedToContainSubstringSolution
{
    private const int AlphabetSize = 26;
    private const int RequiredLCount = 1;
    private const int RequiredECount = 2;
    private const int RequiredTCount = 1;
    private const int OtherLetterCount = AlphabetSize - 3; // every letter but l, e, t

    // dp[a, b, c] = number of length-i strings whose progress toward the
    // requirement is capped at (a, b, c) = (min(count('l'),1), min(count('e'),2),
    // min(count('t'),1)); every other letter is free and does not move the state.
    // Only 2*3*2 = 12 reachable states exist, so this is O(n) regardless of the
    // 26-letter alphabet - the arm the closed form below has to beat.
    public static int CountRearrangeableStringsByStateDp(int n)
    {
        var dp = new long[RequiredLCount + 1, RequiredECount + 1, RequiredTCount + 1];
        dp[0, 0, 0] = 1;

        for (var i = 0; i < n; i++)
        {
            dp = Advance(dp);
        }

        return (int)dp[RequiredLCount, RequiredECount, RequiredTCount];
    }

    private static long[,,] Advance(long[,,] dp)
    {
        var next = new long[RequiredLCount + 1, RequiredECount + 1, RequiredTCount + 1];

        for (var a = 0; a <= RequiredLCount; a++)
        for (var b = 0; b <= RequiredECount; b++)
        for (var c = 0; c <= RequiredTCount; c++)
        {
            var ways = dp[a, b, c];

            if (ways == 0)
            {
                continue;
            }

            Add(next, Math.Min(a + 1, RequiredLCount), b, c, ways);
            Add(next, a, Math.Min(b + 1, RequiredECount), c, ways);
            Add(next, a, b, Math.Min(c + 1, RequiredTCount), ways);
            Add(next, a, b, c, ways * OtherLetterCount);
        }

        return next;
    }

    private static void Add(long[,,] dp, int a, int b, int c, long amount) =>
        dp[a, b, c] = (dp[a, b, c] + amount) % ModularArithmetic.Modulo;

    // Inclusion-exclusion over the 3 ways to fail: A = no 'l' at all, B = fewer
    // than 2 'e's, C = no 't' at all. |A|, |C| and every "no X" alphabet size drop
    // by one letter; B and every term that also caps 'e' at <=1 occurrence uses
    // AtMostOneMarked. answer = 26^n - |A ∪ B ∪ C|, O(log n) via modular
    // exponentiation (Domain.Modular.ModularArithmetic).
    public static int CountRearrangeableStringsByInclusionExclusion(int n)
    {
        var total = ModularArithmetic.Power(AlphabetSize, n);

        var noL = ModularArithmetic.Power(AlphabetSize - 1, n);
        var noT = ModularArithmetic.Power(AlphabetSize - 1, n);
        var shortOnE = AtMostOneMarked(AlphabetSize - 1, n);

        var noLAndNoT = ModularArithmetic.Power(AlphabetSize - 2, n);
        var noLAndShortOnE = AtMostOneMarked(AlphabetSize - 2, n);
        var noTAndShortOnE = AtMostOneMarked(AlphabetSize - 2, n);

        var noLAndNoTAndShortOnE = AtMostOneMarked(AlphabetSize - 3, n);

        var missingSomeRequirement =
            (noL + noT + shortOnE - noLAndNoT - noLAndShortOnE - noTAndShortOnE + noLAndNoTAndShortOnE)
            % ModularArithmetic.Modulo;

        var answer = (total - missingSomeRequirement) % ModularArithmetic.Modulo;

        return (int)((answer + ModularArithmetic.Modulo) % ModularArithmetic.Modulo);
    }

    // Length-n strings drawn from an alphabet of (freeLetters + 1) letters - one
    // "marked" letter plus freeLetters unconstrained ones - that use the marked
    // letter 0 or 1 times: freeLetters^n (0 uses) + n * freeLetters^(n-1) (exactly
    // one position holds it).
    private static long AtMostOneMarked(int freeLetters, int n) =>
        (ModularArithmetic.Power(freeLetters, n)
            + (long)n * ModularArithmetic.Power(freeLetters, n - 1))
        % ModularArithmetic.Modulo;
}
