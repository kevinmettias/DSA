using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestConsecutiveSequence;

public sealed partial class LongestConsecutiveSequenceTests
{
    [Theory]
    [InlineData(new[] { 100, 4, 200, 1, 3, 2 }, 4)]
    [InlineData(new[] { 0,3,7,2,5,8,4,6,0,1 }, 9)]
    public void LongestConsecutive_LeetCodeExamples_ReturnsLongestRun(int[] nums, int expected)
        => Assert.Equal(expected, LongestConsecutive(nums));

    private static int LongestConsecutive(int[] nums)
    {
        var set = new Set<int>();
        foreach (var value in nums) set.TryAdd(value);
        var best = 0;
        foreach (var value in nums)
        {
            if (set.Has(value - 1)) continue;
            var current = value;
            while (set.Has(current)) current++;
            best = Math.Max(best, current - value);
        }
        return best;
    }
}
