using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.LongestBalancedSubarrayI;

// LeetCode 3719. Longest Balanced Subarray I: how long is the longest subarray
// whose distinct-even-value count equals its distinct-odd-value count.
//
// n <= 1500 keeps the intended O(n^2) expanding-window scan comfortably fast, so
// both strategies here share that complexity and differ only in which set
// tracks "already counted" - see LongestBalancedSubarrayII for the
// O(n log^2 n) segment-tree strategy this problem's harder sibling (n <= 1e5)
// actually forces.
internal static class LongestBalancedSubarrayISolution
{
    // The textbook answer: reset a fresh pair of hash sets at every start index
    // and extend the window rightward, counting each distinct value once.
    // Deliberately written without this repo's primitives - the arm the composed
    // strategy below has to justify itself against.
    public static int FindLongestBalancedLengthByBruteForce(int[] nums)
    {
        var longest = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            var evens = new HashSet<int>();
            var odds = new HashSet<int>();

            for (var end = start; end < nums.Length; end++)
            {
                (nums[end] % 2 == 0 ? evens : odds).Add(nums[end]);

                if (evens.Count == odds.Count)
                {
                    longest = Math.Max(longest, end - start + 1);
                }
            }
        }

        return longest;
    }

    // Same O(n^2) expanding-window shape, composed from this repo's own
    // Set<int> (TryAdd's newly-added bool is exactly HashSet.Add's return
    // contract) instead of a hand-rolled BCL HashSet pair.
    public static int FindLongestBalancedLengthByDistinctSetScan(int[] nums)
    {
        var longest = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            var evens = new Set<int>();
            var odds = new Set<int>();

            for (var end = start; end < nums.Length; end++)
            {
                (nums[end] % 2 == 0 ? evens : odds).TryAdd(nums[end]);

                if (evens.Count == odds.Count)
                {
                    longest = Math.Max(longest, end - start + 1);
                }
            }
        }

        return longest;
    }
}
