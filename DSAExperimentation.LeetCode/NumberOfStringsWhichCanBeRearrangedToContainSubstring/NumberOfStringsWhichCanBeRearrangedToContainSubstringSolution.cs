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

    // dp[cappedLCount, cappedECount, cappedTCount] = number of strings of length i
    // whose progress toward the requirement is capped at (min(count('l'), 1),
    // min(count('e'), 2), min(count('t'), 1)); every other letter is free and does
    // not move the state. Only 2*3*2 = 12 reachable states exist, so this is O(n)
    // regardless of the 26-letter alphabet - the arm the closed form below has to beat.
    public static int CountRearrangeableStringsByStateDp(int stringLength)
    {
        var dp = new long[RequiredLCount + 1, RequiredECount + 1, RequiredTCount + 1];
        dp[0, 0, 0] = 1;

        for (var i = 0; i < stringLength; i++)
        {
            dp = Advance(dp);
        }

        return (int)dp[RequiredLCount, RequiredECount, RequiredTCount];
    }

    private static long[,,] Advance(long[,,] dp)
    {
        var next = new long[RequiredLCount + 1, RequiredECount + 1, RequiredTCount + 1];

        for (var cappedLCount = 0; cappedLCount <= RequiredLCount; cappedLCount++)
        {
            for (var cappedECount = 0; cappedECount <= RequiredECount; cappedECount++)
            {
                AdvanceRowOverT(dp, next, cappedLCount, cappedECount);
            }
        }

        return next;
    }

    // One (cappedLCount, cappedECount) row of `dp` advanced across every capped 't'
    // value: each state's ways move into the (cappedLCount + 1, cappedECount,
    // cappedTCount), (cappedLCount, cappedECount + 1, cappedTCount),
    // (cappedLCount, cappedECount, cappedTCount + 1) and
    // (cappedLCount, cappedECount, cappedTCount) destinations. Lifted out of Advance
    // above, which is then only two loops deep.
    private static void AdvanceRowOverT(long[,,] dp, long[,,] next, int cappedLCount, int cappedECount)
    {
        for (var cappedTCount = 0; cappedTCount <= RequiredTCount; cappedTCount++)
        {
            var ways = dp[cappedLCount, cappedECount, cappedTCount];

            if (ways == 0)
            {
                continue;
            }

            Add(next, (A: Math.Min(cappedLCount + 1, RequiredLCount), B: cappedECount, C: cappedTCount), ways);
            Add(next, (A: cappedLCount, B: Math.Min(cappedECount + 1, RequiredECount), C: cappedTCount), ways);
            Add(next, (A: cappedLCount, B: cappedECount, C: Math.Min(cappedTCount + 1, RequiredTCount)), ways);
            Add(next, (A: cappedLCount, B: cappedECount, C: cappedTCount), ways * OtherLetterCount);
        }
    }

    // One (cappedLCount, cappedECount, cappedTCount) pointer into the 3-D table - the
    // capped (l, e, t) progress toward the requirement - so the three coordinates
    // travel as the one state they name rather than as three interchangeable ints.
    private static void Add(long[,,] dp, (int A, int B, int C) state, long amount)
    {
        var (cappedLCount, cappedECount, cappedTCount) = state;
        dp[cappedLCount, cappedECount, cappedTCount] =
            (dp[cappedLCount, cappedECount, cappedTCount] + amount) % ModularArithmetic.Modulo;
    }

    // Inclusion-exclusion over the 3 ways to fail: A = no 'l' at all, B = fewer
    // than 2 'e's, C = no 't' at all. |A|, |C| and every "no X" alphabet size drop
    // by one letter; B and every term that also caps 'e' at <=1 occurrence uses
    // AtMostOneMarked. answer = 26^n - |A ∪ B ∪ C|, O(log n) via modular
    // exponentiation (Domain.Modular.ModularArithmetic).
    public static int CountRearrangeableStringsByInclusionExclusion(int stringLength)
    {
        var total = ModularArithmetic.Power(AlphabetSize, stringLength);

        var noL = ModularArithmetic.Power(AlphabetSize - 1, stringLength);
        var noT = ModularArithmetic.Power(AlphabetSize - 1, stringLength);
        var shortOnE = AtMostOneMarked(AlphabetSize - 1, stringLength);

        var noLAndNoT = ModularArithmetic.Power(AlphabetSize - 2, stringLength);
        var noLAndShortOnE = AtMostOneMarked(AlphabetSize - 2, stringLength);
        var noTAndShortOnE = AtMostOneMarked(AlphabetSize - 2, stringLength);

        var noLAndNoTAndShortOnE = AtMostOneMarked(AlphabetSize - 3, stringLength);

        var missingSomeRequirement =
            (noL + noT + shortOnE - noLAndNoT - noLAndShortOnE - noTAndShortOnE + noLAndNoTAndShortOnE)
            % ModularArithmetic.Modulo;

        var answer = (total - missingSomeRequirement) % ModularArithmetic.Modulo;

        return (int)((answer + ModularArithmetic.Modulo) % ModularArithmetic.Modulo);
    }

    // Strings of length stringLength drawn from an alphabet of (freeLetters + 1)
    // letters - one "marked" letter plus freeLetters unconstrained ones - that use the
    // marked letter 0 or 1 times: freeLetters^stringLength (0 uses) +
    // stringLength * freeLetters^(stringLength - 1) (exactly one position holds it).
    private static long AtMostOneMarked(int freeLetters, int stringLength) =>
        (ModularArithmetic.Power(freeLetters, stringLength)
            + (long)stringLength * ModularArithmetic.Power(freeLetters, stringLength - 1))
        % ModularArithmetic.Modulo;
}
