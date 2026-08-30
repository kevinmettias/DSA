using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MissingNumber;

// LeetCode 268. Missing Number: nums holds n distinct values from
// [0, n], one missing. This repo's own Set<int> (HashMap<Element,bool>-backed)
// records which values are present, then a single scan over [0, n] returns
// the first absent candidate - the same membership-lookup composition
// ContainsDuplicateTests already uses, just for absence instead of presence.
public sealed partial class MissingNumberTests
{
    [Theory]
    [InlineData(new[] { 3, 0, 1 }, 2)]
    [InlineData(new[] { 0, 1 }, 2)]
    [InlineData(new[] { 9, 6, 4, 2, 3, 5, 7, 0, 1 }, 8)]
    public void FindMissingNumber_LeetCodeExamples_ReturnsMissingValue(int[] nums, int expected) =>
        Assert.Equal(expected, FindMissingNumber(nums));

    private static int FindMissingNumber(int[] nums)
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
