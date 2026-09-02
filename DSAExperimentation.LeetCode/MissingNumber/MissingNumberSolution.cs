using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MissingNumber;

// LeetCode 268. Missing Number: nums holds n distinct values from [0, n], one
// missing. Both strategies scan [0, n] for the first candidate not present in
// nums - the textbook O(n^2) "scan the array for each candidate" brute force, or
// this repo's own Set<int> for a one-pass O(n) membership check.
internal static class MissingNumberSolution
{
    // The textbook answer: for each candidate in [0, n], scan the whole array for
    // it. Deliberately written without this repo's primitives - it is the arm the
    // composed solution below has to justify itself against.
    public static int FindMissingNumberByBruteForce(int[] nums)
    {
        for (var candidate = 0; candidate <= nums.Length; candidate++)
        {
            var found = false;

            for (var i = 0; i < nums.Length; i++)
            {
                if (nums[i] == candidate)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return candidate;
            }
        }

        return nums.Length;
    }

    // This repo's own Set<int>: one pass records which values are present, then a
    // single scan over [0, n] returns the first absent candidate.
    public static int FindMissingNumberBySetMembership(int[] nums)
    {
        var present = new Set<int>();

        foreach (var n in nums)
        {
            present.TryAdd(n);
        }

        for (var candidate = 0; candidate <= nums.Length; candidate++)
        {
            if (!present.Has(candidate))
            {
                return candidate;
            }
        }

        return nums.Length;
    }
}
