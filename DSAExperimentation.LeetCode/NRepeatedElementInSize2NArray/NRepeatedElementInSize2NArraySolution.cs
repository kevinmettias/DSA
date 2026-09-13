using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.NRepeatedElementInSize2NArray;

// LeetCode 961. N-Repeated Element in Size 2N Array: the array holds n + 1 distinct
// values across 2n slots, so exactly one value occurs n times - return it.
//
// Both strategies answer the same question with the same signature: the O(n^2)
// pairwise scan that needs nothing but the array, and the single O(n) pass over this
// repo's own Set<int> whose first refused TryAdd is the repeat.
internal static class NRepeatedElementInSize2NArraySolution
{
    private const string NoRepeatedElementFoundMessage =
        "No repeated element found - input violates the problem's own precondition.";

    // The textbook O(n^2) baseline: compare every element against every later one and
    // report the first value that matches. BCL-only by design - this is what you would
    // write without this repo.
    public static int FindRepeatedByPairwiseScan(int[] nums)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                if (nums[i] == nums[j])
                {
                    return nums[i];
                }
            }
        }

        throw new InvalidOperationException(NoRepeatedElementFoundMessage);
    }

    // One O(n) pass over this repo's own Set<int>: the first value TryAdd refuses is
    // already present, and with n + 1 distinct values in 2n slots only the repeated one
    // can ever be seen twice. Same "have I seen this before" role Set<int> plays in
    // NumberOfProvinces, checked on every element instead of only at the end.
    public static int FindRepeatedByTrackingSet(int[] nums)
    {
        var seen = new Set<int>();

        foreach (var value in nums)
        {
            if (!seen.TryAdd(value))
            {
                return value;
            }
        }

        throw new InvalidOperationException(NoRepeatedElementFoundMessage);
    }
}
