using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.SingleNumberIII;

// LeetCode 260. Single Number III: every element appears exactly twice except
// two, which appear exactly once - find both.
//
// Both strategies isolate the two singletons by counting occurrences; they
// differ only in how the count is kept. The composed strategy's whole point
// is to avoid the brute force's repeated inner scan.
internal static class SingleNumberIIISolution
{
    // LC260 guarantees exactly two numbers appear exactly once.
    private const int SingletonCount = 2;

    // The textbook answer: for each element, scan the rest of the array for a
    // duplicate - O(n^2), no lookup structure at all.
    public static int[] FindSingleNumbersByBruteForce(int[] nums)
    {
        var result = new List<int>(SingletonCount);

        for (var i = 0; i < nums.Length; i++)
        {
            var isDuplicate = false;

            for (var j = 0; j < nums.Length; j++)
            {
                if (i != j && nums[i] == nums[j])
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (!isDuplicate)
            {
                result.Add(nums[i]);
            }
        }

        return result.ToArray();
    }

    // This repo's own HashMap<int,int> for a single O(n) frequency-counting pass,
    // then a second pass over the recorded keys for the ones seen exactly once.
    public static int[] FindSingleNumbersByHashMapFrequencyCount(int[] nums)
    {
        var counts = new HashMap<int, int>();

        foreach (var n in nums)
        {
            counts.TryGetValue(n, out var existing);
            counts.Set(n, existing + 1);
        }

        var result = new List<int>(SingletonCount);

        foreach (var key in counts.Keys)
        {
            counts.TryGetValue(key, out var count);

            if (count == 1)
            {
                result.Add(key);
            }
        }

        return result.ToArray();
    }
}
