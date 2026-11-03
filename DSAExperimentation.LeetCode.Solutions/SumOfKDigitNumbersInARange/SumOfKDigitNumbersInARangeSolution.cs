using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.LeetCode.Solutions.SumOfKDigitNumbersInARange;

// LeetCode 3855. Sum of K-Digit Numbers in a Range: every digit of a k-digit
// number is chosen independently from [l, r] (leading zeros allowed when
// l == 0); return the sum of every such number, modulo 1e9+7. k can be as
// large as 1e9, so nothing that enumerates the c^k combinations (c = r-l+1)
// can finish within LeetCode's limits.
//
// SumOfKDigitNumbersByModularRepunit is the closed form: fix any position p
// and sum that position's digit value across all c^k combinations - the other
// k-1 positions range freely, so it factors as (sum of digits l..r) * c^(k-1),
// independent of p. Summing that same quantity over every position's place
// value 10^(k-1-p) collapses to repunit(k) = 111...1 (k ones) = (10^k - 1) / 9.
// Both the modular exponentiation for 10^k and c^(k-1), and the modular
// inverse of 9 that division needs, are this repo's general number-theory
// algorithms, ModularPower and ModularInverse's Fermat inverse, taken modulo
// LeetCodeModulus - the "report modulo 1e9+7" LeetCode convention - and reused
// exactly as FindTheNthValueAfterKSecondsSolution's binomial arm does.
//
// SumOfKDigitNumbersByBruteForceEnumeration is the textbook baseline this has
// to justify itself against: recurse digit-by-digit through every one of the
// c^k combinations, folding each number's value modulo 1e9+7 as it goes.
// Deliberately only tractable for small k.
internal static class SumOfKDigitNumbersInARangeSolution
{
    public static long SumOfKDigitNumbersByBruteForceEnumeration(int low, int high, int digitCount) =>
        BruteForceEnumeration((Low: low, High: high), digitCount, position: 0, valueSoFar: 0);

    public static long SumOfKDigitNumbersByModularRepunit(int low, int high, int digitCount)
    {
        var digitChoices = high - low + 1;
        var digitSum = (low + high) * digitChoices / 2;

        var repunit = (ModularPower.Of(10, digitCount, LeetCodeModulus.Value) - 1) % LeetCodeModulus.Value
            * ModularInverse.ByFermat<LeetCodeModulus>(9) % LeetCodeModulus.Value;
        var combinationsPerDigit = ModularPower.Of(digitChoices, digitCount - 1, LeetCodeModulus.Value);

        return digitSum * combinationsPerDigit % LeetCodeModulus.Value * repunit % LeetCodeModulus.Value;
    }

    // The digits one position may take are a single closed interval: `low` and `high`
    // are chosen together and never mean anything apart, so they travel as one range.
    private static long BruteForceEnumeration(
        (int Low, int High) digitRange, int digitCount, int position, long valueSoFar)
    {
        if (position == digitCount)
        {
            return valueSoFar;
        }

        var total = 0L;
        for (var digit = digitRange.Low; digit <= digitRange.High; digit++)
        {
            var next = (valueSoFar * 10 + digit) % LeetCodeModulus.Value;
            total = (total + BruteForceEnumeration(digitRange, digitCount, position + 1, next))
                % LeetCodeModulus.Value;
        }

        return total;
    }
}
