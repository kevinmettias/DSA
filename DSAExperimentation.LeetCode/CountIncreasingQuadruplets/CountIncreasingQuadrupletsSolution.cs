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
                    for (var l = k + 1; l < n; l++)
                    {
                        if (nums[i] < nums[k] && nums[k] < nums[j] && nums[j] < nums[l])
                        {
                            count++;
                        }
                    }
                }
            }
        }

        return count;
    }

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
            var leftSmallerCount = 0;

            for (var j = 0; j < k; j++)
            {
                if (nums[j] > nums[k])
                {
                    var rightGreaterCount = nums[j] == n ? 0 : suffixGreaterCounts.Query(nums[j], n - 1);
                    total += (long)leftSmallerCount * rightGreaterCount;
                }
                else
                {
                    leftSmallerCount++;
                }
            }

            suffixGreaterCounts.Add(nums[k] - 1, 1);
        }

        return total;
    }
}
