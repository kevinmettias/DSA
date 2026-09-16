using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.LeetCode.MaximumPrimeDifference;

namespace DSAExperimentation.LeetCode.MaximizeCountOfDistinctPrimesAfterSplit;

// LeetCode 3569. Maximize Count of Distinct Primes After Split: nums persists
// updates across queries; after each update, choose a split point k
// (1 <= k < n) maximizing (distinct primes in nums[0..k-1]) plus (distinct
// primes in nums[k..n-1]).
//
// Both strategies share the same Sieve of Eratosthenes over every value
// nums/queries can ever hold - PrimeSieve, declared in MaximumPrimeDifference's
// folder and reused rather than copied, the same arrangement SqrtX's
// SquareExceedsSequence has with FourDivisors, ClosestDivisors and
// ThreeDivisors - and differ only in how they answer "best split" per query:
// the brute-force arm re-scans both sides from scratch with a fresh Set<int>
// for every candidate k, while the composed arm precomputes one prefix pass and
// folds a single suffix pass into it, each side counted through this repo's own
// Set<int>.
internal static class MaximizeCountOfDistinctPrimesAfterSplitSolution
{
    // nums[i] and queries[i][1] are both bounded by this LeetCode constraint.
    private const int MaxValue = 100_000;

    // O(n) work per candidate split, O(n) candidate splits, O(q) queries - the
    // literal reading of the problem, and the arm the prefix/suffix scan below
    // has to beat.
    public static int[] MaxDistinctPrimeCountsByBruteForce(int[] nums, int[][] queries)
    {
        var isComposite = PrimeSieve.BuildCompositeTracker(MaxValue);
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
            var left = CountDistinctPrimes(nums, isComposite, 0, k);
            var right = CountDistinctPrimes(nums, isComposite, k, nums.Length);

            best = Math.Max(best, left + right);
        }

        return best;
    }

    // The distinct primes among nums[start..end), counted from scratch through a fresh
    // Set<int> - the O(n) rescan this arm repeats for each side of each candidate split.
    private static int CountDistinctPrimes(
        int[] nums, DynamicArray<bool> isComposite, int start, int end)
    {
        var primes = new Set<int>();

        for (var i = start; i < end; i++)
        {
            if (!isComposite.Get(nums[i]))
            {
                primes.TryAdd(nums[i]);
            }
        }

        return primes.Count;
    }

    // This repo's own Sieve of Eratosthenes built once, then a single
    // left-to-right pass records the running distinct-prime count at every
    // prefix length, and a single right-to-left pass folds the matching
    // suffix count in as it goes - O(n) per query instead of O(n^2).
    public static int[] MaxDistinctPrimeCountsByPrefixSuffixScan(int[] nums, int[][] queries)
    {
        var isComposite = PrimeSieve.BuildCompositeTracker(MaxValue);
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
        var prefixDistinctPrimeCounts = BuildPrefixDistinctPrimeCounts(nums, isComposite);

        return BestSuffixSplit(nums, isComposite, prefixDistinctPrimeCounts);
    }

    // The single left-to-right pass both halves of the composed arm share:
    // prefixDistinctPrimeCounts[i] is the number of distinct primes among nums[0..i].
    private static int[] BuildPrefixDistinctPrimeCounts(int[] nums, DynamicArray<bool> isComposite)
    {
        var prefixDistinctPrimeCounts = new int[nums.Length];
        var seenFromLeft = new Set<int>();

        for (var i = 0; i < nums.Length; i++)
        {
            if (!isComposite.Get(nums[i]))
            {
                seenFromLeft.TryAdd(nums[i]);
            }

            prefixDistinctPrimeCounts[i] = seenFromLeft.Count;
        }

        return prefixDistinctPrimeCounts;
    }

    // The right-to-left pass that folds the matching suffix count into the prefix one
    // already recorded at the split point, keeping the best total over every k.
    private static int BestSuffixSplit(
        int[] nums, DynamicArray<bool> isComposite, int[] prefixDistinctPrimeCounts)
    {
        var best = 0;
        var seenFromRight = new Set<int>();

        for (var k = nums.Length - 1; k >= 1; k--)
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

}
