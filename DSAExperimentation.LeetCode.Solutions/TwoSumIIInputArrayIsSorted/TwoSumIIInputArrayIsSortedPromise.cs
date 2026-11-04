namespace DSAExperimentation.LeetCode.Solutions.TwoSumIIInputArrayIsSorted;

// LC 167's promise about its input: exactly one pair of indices sums to the target. LeetCode's
// judge never poses an input that breaks it. With several pairs the two-pointer squeeze and the
// binary search each meet a different one first. Generated cases do not always keep the promise, so
// a harness that runs them asks this first.
internal static class TwoSumIIInputArrayIsSortedPromise
{
    public static bool IsKeptBy(int[] nums, int target)
    {
        var counts = nums.GroupBy(value => value).ToDictionary(group => group.Key, group => (long)group.Count());
        var pairs = 0L;

        foreach (var (value, count) in counts)
        {
            var partner = target - value;

            if (partner == value)
            {
                pairs += count * (count - 1) / 2;
            }
            else if (value < partner && counts.TryGetValue(partner, out var partnerCount))
            {
                pairs += count * partnerCount;
            }
        }

        return pairs == 1;
    }
}
