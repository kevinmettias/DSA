using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.CountPrimes;

// LeetCode 204. Count Primes: count the primes strictly less than `limit`.
//
// The two strategies are the classic O(n * sqrt(n)) trial division every
// candidate is checked against, and the O(n log log n) Sieve of Eratosthenes
// that marks composites instead of testing each candidate independently.
internal static class CountPrimesSolution
{
    private const int SmallestCandidate = 2;

    // The textbook answer: test each candidate for a divisor up to its own
    // square root. Deliberately written without this repo's primitives - it
    // is the arm the sieve below has to justify itself against.
    public static int CountPrimesByTrialDivision(int limit)
    {
        var count = 0;

        for (var candidate = SmallestCandidate; candidate < limit; candidate++)
        {
            var isPrime = true;

            for (var divisor = SmallestCandidate; divisor * divisor <= candidate; divisor++)
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
    // starting at its square.
    public static int CountPrimesBySieveOfEratosthenes(int limit)
    {
        if (limit < SmallestCandidate)
        {
            return 0;
        }

        var isComposite = BuildCompositeTracker(limit);
        SieveComposites(isComposite, limit);
        return CountUnmarked(isComposite, limit);
    }

    private static DynamicArray<bool> BuildCompositeTracker(int limit)
    {
        var isComposite = new DynamicArray<bool>();

        for (var i = 0; i < limit; i++)
        {
            isComposite.Add(false);
        }

        return isComposite;
    }

    private static void SieveComposites(DynamicArray<bool> isComposite, int limit)
    {
        for (var i = SmallestCandidate; i * i < limit; i++)
        {
            if (isComposite.Get(i))
            {
                continue;
            }

            for (var multiple = i * i; multiple < limit; multiple += i)
            {
                isComposite.Set(multiple, true);
            }
        }
    }

    private static int CountUnmarked(DynamicArray<bool> isComposite, int limit)
    {
        var count = 0;

        for (var i = SmallestCandidate; i < limit; i++)
        {
            if (!isComposite.Get(i))
            {
                count++;
            }
        }

        return count;
    }
}
