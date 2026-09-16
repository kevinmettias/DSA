using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.ContainsDuplicateII;

// LeetCode 219. Contains Duplicate II: does any value repeat within indexDistance
// positions of itself?
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert them against one example set and the benchmark harness can
// time them against each other without either restating the algorithm.
internal static class ContainsDuplicateIISolution
{
    // The textbook O(n^2) scan of every pair. Correct, and the arm the HashMap
    // strategy has to beat. Deliberately written without this repo's primitives.
    public static bool HasNearbyDuplicateByBruteForce(int[] nums, int indexDistance)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                if (nums[i] == nums[j] && j - i <= indexDistance)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // A single left-to-right pass remembers each value's most recent index in
    // this repo's own HashMap<int,int> and checks the gap the moment a repeat is
    // seen.
    public static bool HasNearbyDuplicateByHashMap(int[] nums, int indexDistance)
    {
        var lastIndex = new HashMap<int, int>();

        for (var i = 0; i < nums.Length; i++)
        {
            if (lastIndex.TryGetValue(nums[i], out var previousIndex) && i - previousIndex <= indexDistance)
            {
                return true;
            }

            lastIndex.Set(nums[i], i);
        }

        return false;
    }
}
