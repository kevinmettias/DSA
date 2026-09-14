using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.MinimumNonZeroProductOfTheArrayElements;

// LeetCode 1969. Minimum Non-Zero Product of the Array Elements: pairing i with
// (2^p-1-i) for every i in [1, 2^p-2] and bit-swapping each pair toward its
// extremes (1 and 2^p-2) minimizes that pair's product while its sum stays fixed;
// the untouched top value 2^p-1 is left alone. The product therefore collapses to
// (2^p-2)^(2^(p-1)-1) * (2^p-1) mod 1e9+7, and the only thing left to choose is how
// that one genuinely large power is raised - `power` below is LeetCode's own p.
//
// Both strategies share the pairing insight and differ only there: repeated modular
// multiplication (O(exponent)) against exponentiation by squaring (O(log exponent)),
// which is Domain.Modular.ModularArithmetic.Power - the same 1e9+7 LeetCode reporting
// convention this repo already declares once. No repo container applies to either
// arm: there is nothing to compose over a handful of running scalars, the same
// "lighter repo-primitive fit" already accepted for Pow(x, n) and Super Pow.
internal static class MinimumNonZeroProductOfTheArrayElementsSolution
{
    // The textbook baseline: multiply the pair value by itself once per pair.
    // Deliberately written without this repo's exponentiation - it is the arm the
    // squaring strategy has to justify itself against, and it only survives at
    // benchmark-sized p, since LeetCode's own p <= 60 puts the exponent past 2^59.
    public static long MinNonZeroProductByRepeatedMultiplication(int power)
    {
        var pairing = BitSwapPairing.Of(power);
        var product = 1L;
        var pairProduct = pairing.PairValue % ModularArithmetic.Modulo;

        for (var pair = 0L; pair < pairing.PairCount; pair++)
        {
            product = product * pairProduct % ModularArithmetic.Modulo;
        }

        return product * (pairing.LargestValue % ModularArithmetic.Modulo) % ModularArithmetic.Modulo;
    }

    // Halve the exponent each step instead of decrementing it, squaring the base to
    // compensate - the same quantity in O(log exponent) multiplications, which is what
    // makes LeetCode's real 2^(p-1)-1 exponents answerable at all.
    public static long MinNonZeroProductBySquaring(int power)
    {
        var pairing = BitSwapPairing.Of(power);

        return ModularArithmetic.Power(pairing.PairValue, pairing.PairCount)
            * (pairing.LargestValue % ModularArithmetic.Modulo) % ModularArithmetic.Modulo;
    }

    // The decomposition both strategies start from, exact for LeetCode's p <= 60
    // because 2^p - 1 still fits in a long.
    private readonly record struct BitSwapPairing(long LargestValue, long PairValue, long PairCount)
    {
        public static BitSwapPairing Of(int power)
        {
            var largestValue = (1L << power) - 1; // 2^p - 1, the value left unpaired
            var pairValue = largestValue - 1; // 2^p - 2, every pair's product once bit-swapped apart
            var pairCount = (1L << (power - 1)) - 1; // 2^(p-1) - 1 pairs, each summing to 2^p - 1

            return new BitSwapPairing(largestValue, pairValue, pairCount);
        }
    }
}
