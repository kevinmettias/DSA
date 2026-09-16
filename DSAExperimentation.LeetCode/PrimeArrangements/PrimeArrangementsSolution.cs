using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.PrimeArrangements;

// LeetCode 1175. Prime Arrangements: count the permutations of 1..upperBound in
// which every prime number sits at a prime index. Primes must fill the prime slots
// and the remaining numbers the rest, and the two groups are independent, so the
// answer is primeCount! * (upperBound - primeCount)! reported modulo 1e9+7.
//
// The permutation count is therefore the same arithmetic either way; the two
// strategies differ only in how the prime count in [1, upperBound] is produced -
// O(n sqrt n) trial division against the O(n log log n) Sieve of Eratosthenes,
// where n is upperBound.
internal static class PrimeArrangementsSolution
{
    // 2 is the smallest prime; every prime-related loop or threshold starts here.
    private const int SmallestPrime = 2;

    // Factorial multiplication only needs to start at 2 - 0! and 1! are both 1.
    private const int FactorialLoopStart = 2;

    // Textbook baseline: test each candidate up to upperBound for a divisor no
    // larger than its own square root. Deliberately written without this repo's
    // primitives - it is the arm the sieve below has to justify itself against.
    public static int CountPrimeArrangementsByTrialDivision(int upperBound) =>
        Arrangements(CountPrimesUpToByTrialDivision(upperBound), upperBound);

    private static int CountPrimesUpToByTrialDivision(int upperBound)
    {
        var count = 0;

        for (var candidate = SmallestPrime; candidate <= upperBound; candidate++)
        {
            var isPrime = true;

            for (var divisor = SmallestPrime; divisor * divisor <= candidate; divisor++)
            {
                if (candidate % divisor == 0)
                {
                    isPrime = false;
                    break;
                }
            }

            if (isPrime)
            {
                count++;
            }
        }

        return count;
    }

    // Sieve of Eratosthenes over this repo's own DynamicArray<bool> as the
    // composite-tracking array, marking multiples of each newly found prime
    // starting at its square - the same composition CountPrimesSolution uses.
    public static int CountPrimeArrangementsBySieveOfEratosthenes(int upperBound) =>
        Arrangements(CountPrimesUpToBySieve(upperBound), upperBound);

    private static int CountPrimesUpToBySieve(int upperBound)
    {
        if (upperBound < SmallestPrime)
        {
            return 0;
        }

        var isComposite = BuildCompositeTracker(upperBound);
        SieveComposites(isComposite, upperBound);
        return CountUnmarked(isComposite, upperBound);
    }

    private static DynamicArray<bool> BuildCompositeTracker(int upperBound)
    {
        var isComposite = new DynamicArray<bool>();

        for (var i = 0; i <= upperBound; i++)
        {
            isComposite.Add(false);
        }

        return isComposite;
    }

    private static void SieveComposites(DynamicArray<bool> isComposite, int upperBound)
    {
        for (var i = SmallestPrime; i * i <= upperBound; i++)
        {
            if (isComposite.Get(i))
            {
                continue;
            }

            for (var multiple = i * i; multiple <= upperBound; multiple += i)
            {
                isComposite.Set(multiple, true);
            }
        }
    }

    private static int CountUnmarked(DynamicArray<bool> isComposite, int upperBound)
    {
        var count = 0;

        for (var i = SmallestPrime; i <= upperBound; i++)
        {
            if (!isComposite.Get(i))
            {
                count++;
            }
        }

        return count;
    }

    // primeCount! * compositeCount! mod 1e9+7 - the whole answer once the prime
    // count is known, shared so the strategies differ only in the counting.
    private static int Arrangements(int primeCount, int upperBound)
    {
        var compositeCount = upperBound - primeCount;

        return (int)(Factorial(primeCount) * Factorial(compositeCount) % ModularArithmetic.Modulo);
    }

    private static long Factorial(int upperBound)
    {
        var result = 1L;

        for (var i = FactorialLoopStart; i <= upperBound; i++)
        {
            result = result * i % ModularArithmetic.Modulo;
        }

        return result;
    }
}
