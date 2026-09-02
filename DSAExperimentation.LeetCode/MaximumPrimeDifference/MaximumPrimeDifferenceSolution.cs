using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.MaximumPrimeDifference;

// LeetCode 3115. Maximum Prime Difference: the maximum distance between the
// indices of two (not necessarily different) prime values in nums.
//
// Both strategies reduce to the same insight - only the FIRST and LAST prime
// index can ever be the far end of the widest pair, so nothing between them
// needs inspecting - but they differ in how each index gets tested for
// primality, the same "trial division vs. precomputed sieve" contrast
// MostFrequentPrimeSolution already draws for LC 3044.
internal static class MaximumPrimeDifferenceSolution
{
    // nums[i] is bounded by this LeetCode constraint, so a sieve only ever
    // needs to cover this many values regardless of how long nums itself is.
    private const int MaxValue = 100;

    // The textbook scan: every pair of prime-valued indices, trial-division
    // tested on the fly. Correct, and the arm the sieve strategy below has to
    // beat - O(n^2) in the number of primes nums actually contains.
    public static int MaxDistanceByBruteForcePairs(int[] nums)
    {
        var best = LeetCodeAnswer.None;

        for (var i = 0; i < nums.Length; i++)
        {
            if (!IsPrimeByTrialDivision(nums[i]))
            {
                continue;
            }

            for (var j = i; j < nums.Length; j++)
            {
                if (IsPrimeByTrialDivision(nums[j]))
                {
                    best = Math.Max(best, j - i);
                }
            }
        }

        return best;
    }

    private static bool IsPrimeByTrialDivision(int value)
    {
        const int SmallestPrime = 2;

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

    // A Sieve of Eratosthenes over [0, MaxValue] built once as a
    // DynamicArray<bool> composite tracker - the same primitive and
    // construction MostFrequentPrimeSolution.MostFrequentPrimeBySieve uses -
    // turns every subsequent primality check into an O(1) lookup, so the
    // single left-to-right/right-to-left scan for the first and last prime
    // index runs in O(n + MaxValue) overall.
    public static int MaxDistanceByEndpointScanWithSieve(int[] nums)
    {
        var isComposite = BuildSieve(MaxValue);
        var first = LeetCodeAnswer.None;
        var last = LeetCodeAnswer.None;

        for (var i = 0; i < nums.Length; i++)
        {
            if (isComposite.Get(nums[i]))
            {
                continue;
            }

            if (first == LeetCodeAnswer.None)
            {
                first = i;
            }

            last = i;
        }

        return first == LeetCodeAnswer.None ? LeetCodeAnswer.None : last - first;
    }

    private static DynamicArray<bool> BuildSieve(int bound)
    {
        var isComposite = new DynamicArray<bool>();

        for (var i = 0; i <= bound; i++)
        {
            isComposite.Add(i < 2);
        }

        for (var i = 2; i * i <= bound; i++)
        {
            if (isComposite.Get(i))
            {
                continue;
            }

            for (var multiple = i * i; multiple <= bound; multiple += i)
            {
                isComposite.Set(multiple, true);
            }
        }

        return isComposite;
    }
}
