using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.SumOfKDigitNumbersInARange;

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
// inverse of 9 that division needs, are this repo's own
// Domain.Modular.ModularArithmetic - the "report modulo 1e9+7" LeetCode
// convention it exists for, reused exactly as
// FindTheNthValueAfterKSecondsSolution's binomial arm does.
//
// SumOfKDigitNumbersByBruteForceEnumeration is the textbook baseline this has
// to justify itself against: recurse digit-by-digit through every one of the
// c^k combinations, folding each number's value modulo 1e9+7 as it goes.
// Deliberately only tractable for small k.
internal static class SumOfKDigitNumbersInARangeSolution
{
    public static long SumOfKDigitNumbersByBruteForceEnumeration(int l, int r, int k) =>
        BruteForceEnumeration(l, r, k, position: 0, valueSoFar: 0);

    private static long BruteForceEnumeration(int l, int r, int k, int position, long valueSoFar)
    {
        if (position == k)
        {
            return valueSoFar;
        }

        var total = 0L;
        for (var digit = l; digit <= r; digit++)
        {
            var next = (valueSoFar * 10 + digit) % ModularArithmetic.Modulo;
            total = (total + BruteForceEnumeration(l, r, k, position + 1, next)) % ModularArithmetic.Modulo;
        }

        return total;
    }

    public static long SumOfKDigitNumbersByModularRepunit(int l, int r, int k)
    {
        var digitCount = r - l + 1;
        var digitSum = (l + r) * digitCount / 2;

        var repunit = (ModularArithmetic.Power(10, k) - 1) % ModularArithmetic.Modulo
            * ModularArithmetic.Inverse(9) % ModularArithmetic.Modulo;
        var combinationsPerDigit = ModularArithmetic.Power(digitCount, k - 1);

        return digitSum * combinationsPerDigit % ModularArithmetic.Modulo * repunit % ModularArithmetic.Modulo;
    }
}
