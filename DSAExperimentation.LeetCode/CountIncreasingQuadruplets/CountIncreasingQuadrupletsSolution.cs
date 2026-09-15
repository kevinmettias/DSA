using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.LeetCode.CountIncreasingQuadruplets;

// LeetCode 2552. Count Increasing Quadruplets: count the index quadruplets
// i < j < k < l with nums[i] < nums[k] < nums[j] < nums[l], where nums is a
// permutation of 1..n.
//
// The condition pivots on the middle "inversion" pair (j, k) - j < k with
// nums[j] > nums[k] - because once that pair is fixed the two outer indices are
// independent: the answer is the sum, over every such pair, of (how many i < j
// have nums[i] < nums[k]) times (how many l > k have nums[l] > nums[j]). The two
// strategies differ only in how they obtain the second factor.
internal static class CountIncreasingQuadrupletsSolution
{
    // The textbook answer: four nested loops testing the inequality exactly as
    // the statement writes it. Deliberately written without this repo's
    // primitives - no pivot, no counting structure - it is the arm the composed
    // solution below has to justify itself against. O(n^4).
    public static long CountQuadrupletsByBruteForce(int[] nums)
    {
        var n = nums.Length;
        var count = 0L;

        for (var i = 0; i < n; i++)
        {
            for (var j = i + 1; j < n; j++)
            {
                for (var k = j + 1; k < n; k++)
                {
                    count += CountQuadrupletsWithFirstThree(nums, i, j, k);
                }
            }
        }

        return count;
    }

    // The fourth index l > k: the triple (i, j, k) must already be the inverted
    // inner pair and nums[l] must rise above nums[j]. Lifted out of the brute-force
    // scan above, which is then only three loops deep.
    private static long CountQuadrupletsWithFirstThree(int[] nums, int i, int j, int k)
    {
        var count = 0L;

        for (var l = k + 1; l < nums.Length; l++)
        {
            if (IsInvertedInnerPair(nums, i, j, k) && nums[j] < nums[l])
            {
                count++;
            }
        }

        return count;
    }

    // The inversion the statement pivots on: nums[i] < nums[k] < nums[j], the pair
    // (j, k) inverting with nums[i] resting below the pivot.
    private static bool IsInvertedInnerPair(int[] nums, int i, int j, int k)
        => nums[i] < nums[k] && HasInvertedPivotPair(nums, j, k);

    // Sweep k from the right. The first factor needs no structure at all: its
    // threshold nums[k] is fixed for the whole inner j-loop, so a running counter
    // of the j's already passed with nums[j] < nums[k] is exact. The second factor
    // is a rank query over the suffix, which is what this repo's own
    // FenwickTree<int, SumOperation<int>> - a Binary Indexed Tree of the values
    // already admitted from the right - answers in O(log n): value v is stored at
    // index v-1, so Query(nums[j], n-1) counts the admitted values strictly
    // greater than nums[j] (and nums[j] == n has nothing above it). O(n^2 log n),
    // the same "Fenwick tree of counts swept alongside a value-rank query" shape
    // LC 2426 and LC 315 use, here swept right to left because the query side
    // (l > k) is a suffix rather than a prefix.
    public static long CountQuadrupletsByFenwickTreeSweep(int[] nums)
    {
        var n = nums.Length;
        var suffixGreaterCounts = new FenwickTree<int, SumOperation<int>>(n);
        var total = 0L;

        for (var k = n - 1; k >= 0; k--)
        {
            total += CountPivotPairsAt(nums, k, suffixGreaterCounts);
            suffixGreaterCounts.Add(nums[k] - 1, 1);
        }

        return total;
    }

    // Every j < k inverting with nums[k] contributes the j's seen so far below
    // nums[k] times the suffix values above nums[j] admitted by the Fenwick tree.
    private static long CountPivotPairsAt(
        int[] nums, int k, FenwickTree<int, SumOperation<int>> suffixGreaterCounts)
    {
        var n = nums.Length;
        var leftSmallerCount = 0;
        var pairTotal = 0L;

        for (var j = 0; j < k; j++)
        {
            if (HasInvertedPivotPair(nums, j, k))
            {
                // nums is a permutation of 1..n, so a value of n has nothing above it
                // in the suffix.
                var isLargestValue = nums[j] == n;
                var rightGreaterCount = isLargestValue ? 0 : suffixGreaterCounts.Query(nums[j], n - 1);
                pairTotal += (long)leftSmallerCount * rightGreaterCount;
            }
            else
            {
                leftSmallerCount++;
            }
        }

        return pairTotal;
    }

    // The middle pair inverts: j sits before k but holds the larger value. Both
    // arms pivot on exactly this test, so it is stated once.
    private static bool HasInvertedPivotPair(int[] nums, int j, int k) => nums[j] > nums[k];
}
