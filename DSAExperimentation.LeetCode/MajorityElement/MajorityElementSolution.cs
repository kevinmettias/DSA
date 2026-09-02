using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.MajorityElement;

// LeetCode 169. Majority Element: the array is guaranteed to have an element
// appearing more than n/2 times, so a running count can return as soon as one
// candidate crosses that threshold instead of scanning to the end.
internal static class MajorityElementSolution
{
    public static int MajorityByHashMap(int[] nums)
    {
        var counts = new HashMap<int, int>();

        foreach (var n in nums)
        {
            counts.TryGetValue(n, out var count);
            count++;

            if (count > nums.Length / 2)
            {
                return n;
            }

            counts.Set(n, count);
        }

        return nums[0];
    }
}
