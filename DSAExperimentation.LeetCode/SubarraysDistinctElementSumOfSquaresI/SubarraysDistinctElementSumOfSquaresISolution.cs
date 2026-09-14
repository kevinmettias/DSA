using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.SubarraysDistinctElementSumOfSquaresI;

// LeetCode 2913. Subarrays Distinct Element Sum of Squares I: sum, over every
// subarray [l, r], of distinct(l, r)^2. Unlike part II (LC 2916) there is no
// modulus - n <= 100 by the problem's own constraints, so the exact sum fits a
// long and LeetCode wants it unreduced.
//
// Both strategies answer that question with the same signature, so the test
// harness can assert both against LeetCode's examples and the benchmark harness
// can time them against each other without either restating the algorithm.
internal static class SubarraysDistinctElementSumOfSquaresISolution
{
    // Textbook O(n^3): for every subarray, re-derive its distinct count from
    // scratch by scanning the prefix before each element to see whether that value
    // has already occurred. Deliberately BCL-only - not even a hash set - because
    // it is the arm the growing-set strategy has to justify itself against.
    public static long SumOfSquaresByBruteForce(int[] nums)
    {
        var answer = 0L;

        for (var start = 0; start < nums.Length; start++)
        {
            for (var end = start; end < nums.Length; end++)
            {
                var distinct = CountDistinctByLinearScan(nums, start, end);
                answer += (long)distinct * distinct;
            }
        }

        return answer;
    }

    private static int CountDistinctByLinearScan(int[] nums, int start, int end)
    {
        var distinct = 0;

        for (var index = start; index <= end; index++)
        {
            if (IsFirstOccurrence(nums, start, index))
            {
                distinct++;
            }
        }

        return distinct;
    }

    private static bool IsFirstOccurrence(int[] nums, int start, int index)
    {
        for (var earlier = start; earlier < index; earlier++)
        {
            if (nums[earlier] == nums[index])
            {
                return false;
            }
        }

        return true;
    }

    // Composed: fix the start index and sweep the end index rightward, keeping one
    // Set<int> of the values seen so far. Extending the subarray by one element is
    // a single amortized-O(1) TryAdd instead of a fresh scan, and Set<int>.Count is
    // the distinct count outright - O(n^2) overall, the same complexity split
    // TwoSumSolution's brute-force-versus-HashMap arms demonstrate.
    public static long SumOfSquaresByGrowingSet(int[] nums)
    {
        var answer = 0L;

        for (var start = 0; start < nums.Length; start++)
        {
            var distinct = new Set<int>();

            for (var end = start; end < nums.Length; end++)
            {
                distinct.TryAdd(nums[end]);
                answer += (long)distinct.Count * distinct.Count;
            }
        }

        return answer;
    }
}
