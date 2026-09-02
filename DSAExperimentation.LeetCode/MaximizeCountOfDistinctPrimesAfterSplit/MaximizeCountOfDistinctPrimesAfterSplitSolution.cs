using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MaximizeCountOfDistinctPrimesAfterSplit;

// LeetCode 3569. Maximize Count of Distinct Primes After Split: nums persists
// updates across queries; after each update, choose a split point k
// (1 <= k < n) maximizing (distinct primes in nums[0..k-1]) plus (distinct
// primes in nums[k..n-1]).
//
// Both strategies share the same Sieve of Eratosthenes construction
// MaximumPrimeDifferenceSolution/MostFrequentPrimeSolution already use (a
// DynamicArray<bool> composite tracker over every value nums/queries can ever
// hold - duplicated per file rather than pulled into a shared helper, the same
// precedent those two solutions already set) and only differ in how they
// answer "best split" per query: the brute-force arm re-scans both sides from
// scratch with a fresh Set<int> for every candidate k, while the composed arm
// precomputes one prefix pass and folds a single suffix pass into it, each
// side counted through this repo's own Set<int>.
internal static class MaximizeCountOfDistinctPrimesAfterSplitSolution
{
    // nums[i] and queries[i][1] are both bounded by this LeetCode constraint.
    private const int MaxValue = 100_000;

    // O(n) work per candidate split, O(n) candidate splits, O(q) queries - the
    // literal reading of the problem, and the arm the prefix/suffix scan below
    // has to beat.
    public static int[] MaxDistinctPrimeCountsByBruteForce(int[] nums, int[][] queries)
    {
        var isComposite = BuildSieve(MaxValue);
        var answers = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            nums[queries[q][0]] = queries[q][1];
            answers[q] = BestSplitByRescanningEachSide(nums, isComposite);
        }

        return answers;
    }

    private static int BestSplitByRescanningEachSide(int[] nums, DynamicArray<bool> isComposite)
    {
        var best = 0;

        for (var k = 1; k < nums.Length; k++)
        {
            var prefixPrimes = new Set<int>();

            for (var i = 0; i < k; i++)
            {
                if (!isComposite.Get(nums[i]))
                {
                    prefixPrimes.TryAdd(nums[i]);
                }
            }

            var suffixPrimes = new Set<int>();

            for (var i = k; i < nums.Length; i++)
            {
                if (!isComposite.Get(nums[i]))
                {
                    suffixPrimes.TryAdd(nums[i]);
                }
            }

            best = Math.Max(best, prefixPrimes.Count + suffixPrimes.Count);
        }

        return best;
    }

    // This repo's own Sieve of Eratosthenes built once, then a single
    // left-to-right pass records the running distinct-prime count at every
    // prefix length, and a single right-to-left pass folds the matching
    // suffix count in as it goes - O(n) per query instead of O(n^2).
    public static int[] MaxDistinctPrimeCountsByPrefixSuffixScan(int[] nums, int[][] queries)
    {
        var isComposite = BuildSieve(MaxValue);
        var answers = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            nums[queries[q][0]] = queries[q][1];
            answers[q] = BestSplitByPrefixSuffixScan(nums, isComposite);
        }

        return answers;
    }

    private static int BestSplitByPrefixSuffixScan(int[] nums, DynamicArray<bool> isComposite)
    {
        var n = nums.Length;
        var prefixDistinctPrimeCounts = new int[n];
        var seenFromLeft = new Set<int>();

        for (var i = 0; i < n; i++)
        {
            if (!isComposite.Get(nums[i]))
            {
                seenFromLeft.TryAdd(nums[i]);
            }

            prefixDistinctPrimeCounts[i] = seenFromLeft.Count;
        }

        var best = 0;
        var seenFromRight = new Set<int>();

        for (var k = n - 1; k >= 1; k--)
        {
            if (!isComposite.Get(nums[k]))
            {
                seenFromRight.TryAdd(nums[k]);
            }

            var total = prefixDistinctPrimeCounts[k - 1] + seenFromRight.Count;
            best = Math.Max(best, total);
        }

        return best;
    }

    // Mirrors MaximumPrimeDifferenceSolution.BuildSieve/MostFrequentPrimeSolution.
    // BuildSieve exactly.
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
