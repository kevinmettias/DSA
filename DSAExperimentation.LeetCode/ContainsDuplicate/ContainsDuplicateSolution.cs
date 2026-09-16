using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.ContainsDuplicate;

// LeetCode 217. Contains Duplicate: true iff any value appears more than once.
//
// The two strategies differ only in how "have I seen this value before" is
// answered: brute force compares every pair directly; the set-probe strategy
// records each value in this repo's own Set<int> and short-circuits the moment a
// value fails to be newly added.
internal static class ContainsDuplicateSolution
{
    public static bool HasDuplicateByBruteForce(int[] nums)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                if (nums[i] == nums[j])
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool HasDuplicateBySetProbe(int[] nums)
    {
        var seen = new Set<int>();

        foreach (var value in nums)
        {
            if (!seen.TryAdd(value))
            {
                return true;
            }
        }

        return false;
    }
}
