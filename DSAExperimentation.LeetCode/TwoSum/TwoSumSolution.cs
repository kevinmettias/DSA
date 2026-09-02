using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.TwoSum;

// LeetCode 1. Two Sum: find the two indices whose values sum to target.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class TwoSumSolution
{
    // The textbook O(n^2) scan of every unordered pair. Correct, and the arm the
    // HashMap strategy has to beat.
    public static bool TryFindIndicesByBruteForce(int[] nums, int target, out int first, out int second)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                if (nums[i] + nums[j] == target)
                {
                    first = i;
                    second = j;
                    return true;
                }
            }
        }

        first = 0;
        second = 0;
        return false;
    }

    // One O(n) pass over this repo's own HashMap<TKey, TValue>: each value is
    // recorded against its index, so the complement of a later value is a single
    // lookup away.
    public static bool TryFindIndicesByHashMap(int[] nums, int target, out int first, out int second)
    {
        var seen = new HashMap<int, int>();

        for (var i = 0; i < nums.Length; i++)
        {
            if (seen.TryGetValue(target - nums[i], out var matchIndex))
            {
                first = matchIndex;
                second = i;
                return true;
            }

            seen.Set(nums[i], i);
        }

        first = 0;
        second = 0;
        return false;
    }
}
