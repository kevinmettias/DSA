using DSAExperimentation.Algorithms.NumberTheory;
using DSAExperimentation.Domain.Language.English;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.LeetCode.Solutions.NumberOfStringsWhichCanBeRearrangedToContainSubstring;

// LeetCode 2930. Number of Strings Which Can Be Rearranged to Contain Substring
// "leet": count length-n lowercase strings (mod 1e9+7) that have SOME permutation
// containing "leet". Rearrangement is free, so this is purely a question about
// letter counts: a string qualifies exactly when it has at least one 'l', at
// least two 'e's and at least one 't' - any such multiset can always be arranged
// as "leet" followed by the rest in any order.
[LeetCodeQuestion("number-of-strings-which-can-be-rearranged-to-contain-substring", LeetCodeAnswerOrder.AsWritten)]
internal static class NumberOfStringsWhichCanBeRearrangedToContainSubstringSolution
{
    private const int RequiredLCount = 1;
    private const int RequiredECount = 2;
    private const int RequiredTCount = 1;
    private const int OtherLetterCount = EnglishLetters.Size - 3; // every letter but l, e, t

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
            (dp[cappedLCount, cappedECount, cappedTCount] + amount) % LeetCodeModulus.Value;
    }

    // Inclusion-exclusion over the 3 ways to fail: A = no 'l' at all, B = fewer
    // than 2 'e's, C = no 't' at all. |A|, |C| and every "no X" alphabet size drop
    // by one letter; B and every term that also caps 'e' at <=1 occurrence uses
    // AtMostOneMarked. answer = 26^n - |A ∪ B ∪ C|, O(log n) via modular
    // exponentiation (ModularPower).
    public static int CountRearrangeableStringsByInclusionExclusion(int stringLength)
    {
        var total = ModularPower.Of(EnglishLetters.Size, stringLength, LeetCodeModulus.Value);

        var noL = ModularPower.Of(EnglishLetters.Size - 1, stringLength, LeetCodeModulus.Value);
        var noT = ModularPower.Of(EnglishLetters.Size - 1, stringLength, LeetCodeModulus.Value);
        var shortOnE = AtMostOneMarked(EnglishLetters.Size - 1, stringLength);

        var noLAndNoT = ModularPower.Of(EnglishLetters.Size - 2, stringLength, LeetCodeModulus.Value);
        var noLAndShortOnE = AtMostOneMarked(EnglishLetters.Size - 2, stringLength);
        var noTAndShortOnE = AtMostOneMarked(EnglishLetters.Size - 2, stringLength);

        var noLAndNoTAndShortOnE = AtMostOneMarked(EnglishLetters.Size - 3, stringLength);

        var missingSomeRequirement =
            (noL + noT + shortOnE - noLAndNoT - noLAndShortOnE - noTAndShortOnE + noLAndNoTAndShortOnE)
            % LeetCodeModulus.Value;

        var answer = (total - missingSomeRequirement) % LeetCodeModulus.Value;

        return (int)((answer + LeetCodeModulus.Value) % LeetCodeModulus.Value);
    }

    // Strings of length stringLength drawn from an alphabet of (freeLetters + 1)
    // letters - one "marked" letter plus freeLetters unconstrained ones - that use the
    // marked letter 0 or 1 times: freeLetters^stringLength (0 uses) +
    // stringLength * freeLetters^(stringLength - 1) (exactly one position holds it).
    private static long AtMostOneMarked(int freeLetters, int stringLength) =>
        (ModularPower.Of(freeLetters, stringLength, LeetCodeModulus.Value)
            + (long)stringLength * ModularPower.Of(freeLetters, stringLength - 1, LeetCodeModulus.Value))
        % LeetCodeModulus.Value;
}
