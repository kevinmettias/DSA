using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HouseRobberIV;

// LeetCode 2560. House Robber IV: "binary search on the answer" over the robber's
// capability - the smallest cap that admits robbing >= k non-adjacent houses each
// worth at most cap is monotone (once some cap is feasible, every larger cap stays
// feasible), so it's the leftmost "true" in an implicit [false...false, true...true]
// sequence over cap in [min(nums), max(nums)]. Same FeasibleCapabilitySequence +
// BinarySearch.LowerBound shape KokoEatingBananasTests/SplitArrayLargestSumTests
// already use for their own search-on-answer.
public sealed partial class HouseRobberIVTests
{
    [Theory]
    [InlineData(new[] { 2, 3, 5, 9 }, 2, 5)]
    [InlineData(new[] { 2, 7, 9, 3, 1 }, 2, 2)]
    public void MinCapability_LeetCodeExamples_ReturnsSmallestFeasibleCapability(int[] nums, int k, int expected)
    {
        var actual = MinCapability(nums, k);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void MinCapability_KEqualsHouseCount_ReturnsMaxValue()
    {
        int[] nums = [6, 1, 8];

        var actual = MinCapability(nums, k: 2);

        Assert.Equal(8, actual);
    }

    private static int MinCapability(int[] nums, int k)
    {
        var floor = nums.Min();
        var ceiling = nums.Max();
        var sequence = new FeasibleCapabilitySequence(nums, k, floor, ceiling);

        return floor + BinarySearch.LowerBound(sequence, true);
    }

    private static bool CanRobAtLeastKWithinCap(int[] nums, int k, int cap)
    {
        var count = 0;
        var previousRobbed = false;

        foreach (var value in nums)
        {
            if (value <= cap && !previousRobbed)
            {
                count++;
                previousRobbed = true;
            }
            else
            {
                previousRobbed = false;
            }
        }

        return count >= k;
    }

    private readonly struct FeasibleCapabilitySequence(int[] nums, int k, int floor, int ceiling) : IRandomAccessSequence<bool>
    {
        public int Length => ceiling - floor + 1;

        public bool Get(int index) => CanRobAtLeastKWithinCap(nums, k, floor + index);
    }
}
