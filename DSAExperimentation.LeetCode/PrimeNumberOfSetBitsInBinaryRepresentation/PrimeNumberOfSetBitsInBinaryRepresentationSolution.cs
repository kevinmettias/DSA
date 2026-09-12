using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.PrimeNumberOfSetBitsInBinaryRepresentation;

// LeetCode 762. Prime Number of Set Bits in Binary Representation: count how many
// values in [left, right] have a prime popcount.
//
// right <= 10^6 bounds every candidate's popcount to at most 20, so "is this bit
// count prime" is membership in the fixed small set of primes <= 20, not a general
// primality test - the two strategies differ only in whether that membership check
// is re-derived (trial division) per value or looked up in a set built once.
internal static class PrimeNumberOfSetBitsInBinaryRepresentationSolution
{
    private const int SmallestPrime = 2;
    private static readonly int[] PrimesUpToTwenty = { 2, 3, 5, 7, 11, 13, 17, 19 };
    private static readonly Set<int> PrimeBitCounts = BuildPrimeBitCounts();

    // The textbook answer: trial-divide each popcount for primality on the spot,
    // written without this repo's primitives - the arm the set-lookup strategy
    // below has to justify itself against.
    public static int CountPrimeSetBitsByTrialDivision(int left, int right)
    {
        var count = 0;

        for (var value = left; value <= right; value++)
        {
            if (IsPrime(CountSetBits(value)))
            {
                count++;
            }
        }

        return count;
    }

    // Every reachable popcount is looked up in this repo's own Set<int>
    // (HashMap<Element,bool>-backed), built once, instead of re-deriving
    // primality per value.
    public static int CountPrimeSetBitsByPrecomputedSet(int left, int right)
    {
        var count = 0;

        for (var value = left; value <= right; value++)
        {
            if (PrimeBitCounts.Has(CountSetBits(value)))
            {
                count++;
            }
        }

        return count;
    }

    private static int CountSetBits(int value)
    {
        var bits = 0;

        while (value != 0)
        {
            value &= value - 1;
            bits++;
        }

        return bits;
    }

    private static bool IsPrime(int value)
    {
        if (value < SmallestPrime)
        {
            return false;
        }

        for (var divisor = SmallestPrime; divisor * divisor <= value; divisor++)
        {
            if (value % divisor == 0)
            {
                return false;
            }
        }

        return true;
    }

    private static Set<int> BuildPrimeBitCounts()
    {
        var primes = new Set<int>();

        foreach (var candidate in PrimesUpToTwenty)
        {
            primes.TryAdd(candidate);
        }

        return primes;
    }
}
