using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.ElementAlgebra;
using DSAExperimentation.DataStructures.PrefixSums;

namespace DSAExperimentation.LeetCode.MinimumSizeSubarraySum;

// LeetCode 209. Minimum Size Subarray Sum: length of the shortest contiguous
// subarray of a positive-integer array whose sum is >= target, or 0 if no such
// subarray exists.
//
// nums are all positive, so the running prefix-sum totals are strictly increasing -
// already "sorted ascending" in exactly the shape this repo's own
// BinarySearch.LowerBound assumes. For each start index i, LowerBound over
// PrefixSums' Totals view finds the smallest end index whose cumulative sum
// first reaches target - an O(n log n) alternative to the textbook O(n)
// two-pointer/every-subarray scan, composing two existing production primitives
// (BinarySearch.LowerBound, PrefixSums) instead of a hand-rolled scan, the
// same search-on-a-derived-monotonic-sequence idiom
// MedianOfTwoSortedArraysSolutionTests already established.
internal static class MinimumSizeSubarraySumSolution
{
    // The textbook baseline: every subarray start i, extending right until the sum
    // first reaches target, O(n^2) worst case. Deliberately written without this
    // repo's primitives - it is the arm the composed strategy below has to justify
    // itself against.
    public static int MinLengthByBruteForce(int target, int[] nums)
    {
        var best = int.MaxValue;

        for (var i = 0; i < nums.Length; i++)
        {
            var sum = 0;

            for (var j = i; j < nums.Length; j++)
            {
                sum += nums[j];

                if (sum >= target)
                {
                    best = Math.Min(best, j - i + 1);
                    break;
                }
            }
        }

        return best == int.MaxValue ? 0 : best;
    }

    // The composed answer: PrefixSums' running totals, binary-searched via
    // BinarySearch.LowerBound for each start index's earliest qualifying end.
    public static int MinLengthByBinarySearchPrefixSum(int target, int[] nums)
    {
        var prefix = new PrefixSums<int, SumOperation<int>>(nums);

        var best = int.MaxValue;

        for (var i = 0; i < nums.Length; i++)
        {
            var end = BinarySearch.LowerBound(prefix.Totals, target + prefix.TotalBefore(i));

            if (end <= nums.Length)
            {
                best = Math.Min(best, end - i);
            }
        }

        return best == int.MaxValue ? 0 : best;
    }
}
