using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TwoSum;

// LeetCode 1. Two Sum: one O(n) pass over this repo's own HashMap<TValue,TIndex>,
// not the textbook O(n^2) brute force.
public sealed partial class TwoSumTests
{
    [Fact]
    public void FindIndices_ClassicExample_ReturnsMatchingPairIndices()
    {
        int[] nums = [2, 7, 11, 15];

        var found = TryFindTwoSumIndices(nums, target: 9, out var first, out var second);

        Assert.True(found);
        Assert.Equal(0, first);
        Assert.Equal(1, second);
    }

    [Fact]
    public void FindIndices_NoPairSumsToTarget_ReturnsFalse()
    {
        int[] nums = [1, 2, 3];

        var found = TryFindTwoSumIndices(nums, target: 100, out _, out _);

        Assert.False(found);
    }

    private static bool TryFindTwoSumIndices(int[] nums, int target, out int first, out int second)
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
