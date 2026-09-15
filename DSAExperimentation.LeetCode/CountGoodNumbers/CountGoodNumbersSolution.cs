using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountGoodNumbers;

// LeetCode 1922. Count Good Numbers: a digit string of length n is "good" when every
// even index holds one of the 5 even digits and every odd index holds one of the 4
// prime digits, so the count is 5^ceil(n/2) * 4^floor(n/2) modulo 1e9+7.
//
// Both strategies compute exactly that product and differ only in how a power is
// raised: once per unit of exponent, or by squaring. n can be up to 1e15, so the
// first arm is the textbook shape the second has to justify itself against - and at
// LeetCode's real bounds only the second one finishes at all.
//
// No repo container or algorithm primitive applies to the surrounding arithmetic -
// there is nothing to compose over two running scalars (result/base), the same
// "lighter repo-primitive fit" this repo already accepted for Pow(x, n) and Super
// Pow. The modulus and the squaring loop are Domain.Modular's, because 1e9+7 is
// LeetCode's reporting convention rather than this problem's own value.
internal static class CountGoodNumbersSolution
{
    // LC 1922: even-index digits must be one of {0,2,4,6,8}, odd-index digits must
    // be one of the prime digits {2,3,5,7}.
    private const long EvenIndexDigitChoices = 5;
    private const long OddIndexDigitChoices = 4;

    // Splits the n positions into how many sit at an even index vs. an odd index.
    private const int PositionParityDivisor = 2;

    // The textbook baseline: multiply by the digit-choice count once per position,
    // reducing mod 1e9+7 after each multiplication so nothing overflows.
    // Deliberately written without this repo's primitives - it is the O(n) arm the
    // squaring strategy below has to justify itself against.
    public static int CountGoodNumbersByRepeatedMultiplication(long n)
    {
        var (evenPositions, oddPositions) = SplitByIndexParity(n);
        var evenChoices = NaivePower(EvenIndexDigitChoices, evenPositions);
        var oddChoices = NaivePower(OddIndexDigitChoices, oddPositions);

        return (int)(evenChoices * oddChoices % ModularArithmetic.Modulo);
    }

    private static long NaivePower(long value, long exponent)
    {
        var result = 1L;

        for (var i = 0L; i < exponent; i++)
        {
            result = result * value % ModularArithmetic.Modulo;
        }

        return result;
    }

    // Halve the exponent each step instead of decrementing it, squaring the base to
    // compensate - Domain.Modular's own exponentiation-by-squaring loop, which folds
    // under the modulus at every multiplication so intermediate values never grow
    // past Modulo^2. O(log n) instead of O(n).
    public static int CountGoodNumbersByExponentiationBySquaring(long n)
    {
        var (evenPositions, oddPositions) = SplitByIndexParity(n);
        var evenChoices = ModularArithmetic.Power(EvenIndexDigitChoices, evenPositions);
        var oddChoices = ModularArithmetic.Power(OddIndexDigitChoices, oddPositions);

        return (int)(evenChoices * oddChoices % ModularArithmetic.Modulo);
    }

    // Index 0 is even, so a length-n string carries ceil(n/2) even-index positions
    // and floor(n/2) odd-index ones.
    private static (long EvenPositions, long OddPositions) SplitByIndexParity(long n)
        => ((n + 1) / PositionParityDivisor, n / PositionParityDivisor);
}
