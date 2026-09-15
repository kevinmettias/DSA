using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountTheNumberOfGoodPartitions;

// LeetCode 2963. Count the Number of Good Partitions: split nums into one or more
// contiguous subarrays so that no value appears in two different subarrays; count
// the ways to do that, modulo 1e9+7.
//
// A partition is "good" exactly when every cut falls outside every value's own
// occurrence span. Walking the array while tracking the furthest last-occurrence
// seen so far finds every index that is forced to close a span - those forced
// closes are the only places every good partition may cut, and they are
// independent of each other, so the count is 2^(forced closes - 1): every
// non-final forced close is either cut or merged into the next span.
internal static class CountTheNumberOfGoodPartitionsSolution
{
    // Textbook baseline: enumerate all 2^(n-1) ways to place a cut between each
    // pair of adjacent elements, and check each one directly against the
    // definition - no value's occurrences land in two different groups. Correct
    // but exponential in the array length, which is exactly what the
    // last-occurrence-merge strategy below has to be measured against.
    public static long CountGoodPartitionsByBruteForce(int[] nums)
    {
        var cutCount = 1 << (nums.Length - 1);
        var validPartitions = 0L;

        for (var mask = 0; mask < cutCount; mask++)
        {
            if (IsGoodPartition(nums, mask))
            {
                validPartitions++;
            }
        }

        return validPartitions % ModularArithmetic.Modulo;
    }

    private static bool IsGoodPartition(int[] nums, int cutMask)
    {
        var groupId = 0;
        var groupOfValue = new Dictionary<int, int>();

        for (var i = 0; i < nums.Length; i++)
        {
            if (i > 0 && (cutMask & (1 << (i - 1))) != 0)
            {
                groupId++;
            }

            if (IsInAnotherGroup(groupOfValue, nums[i], groupId))
            {
                return false;
            }
        }

        return true;
    }

    // Whether `value` already belongs to a group other than `groupId` - the one
    // thing that makes a partition bad. A value not seen before joins `groupId`
    // and is by definition fine.
    private static bool IsInAnotherGroup(Dictionary<int, int> groupOfValue, int value, int groupId)
    {
        if (!groupOfValue.TryGetValue(value, out var existingGroup))
        {
            groupOfValue[value] = groupId;

            return false;
        }

        return existingGroup != groupId;
    }

    // One O(n) scan finds every forced-close index by tracking the furthest
    // last-occurrence seen so far (the same "merge overlapping spans" shape as
    // interval-merging), then Domain.Modular.ModularArithmetic.Power reports
    // 2^(forced closes - 1) mod 1e9+7 - no partition is ever materialized.
    public static long CountGoodPartitionsByLastOccurrenceMerge(int[] nums)
    {
        var lastOccurrence = new Dictionary<int, int>();

        for (var i = 0; i < nums.Length; i++)
        {
            lastOccurrence[nums[i]] = i;
        }

        var forcedCloses = 0;
        var spanEnd = -1;

        for (var i = 0; i < nums.Length; i++)
        {
            spanEnd = Math.Max(spanEnd, lastOccurrence[nums[i]]);

            if (i == spanEnd)
            {
                forcedCloses++;
            }
        }

        return ModularArithmetic.Power(2, forcedCloses - 1);
    }
}
