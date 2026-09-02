using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ChalkboardXorGame;

// LeetCode 810. Chalkboard XOR Game: full minimax recursion over which subset of
// indices remains on the board (a bitmask), via this repo's own Memoizer - the same
// "recursion reduces to a closed form" shape NimGameTests.cs already uses, here
// reducing to the well-known (xor(nums) == 0) || (nums.Length % 2 == 0) theorem the
// benchmark compares against.
public sealed partial class ChalkboardXorGameTests
{
    [Theory]
    [InlineData(new[] { 1, 1, 2 }, false)]
    [InlineData(new[] { 0, 1 }, true)]
    [InlineData(new[] { 1, 2, 3 }, true)]
    public void CurrentPlayerCanWin_LeetCodeExamples_ReturnsWhetherAliceWins(int[] nums, bool expected)
        => Assert.Equal(expected, AliceWins(nums));

    private static bool AliceWins(int[] nums)
    {
        var fullMask = (1 << nums.Length) - 1;
        return Memoizer.Memoize<int, bool>(fullMask, (mask, currentPlayerWins) => CurrentPlayerWins(mask, currentPlayerWins, nums));
    }

    private static bool CurrentPlayerWins(int mask, Func<int, bool> currentPlayerWins, int[] nums)
    {
        if (XorOf(mask, nums) == 0)
        {
            return true;
        }

        for (var i = 0; i < nums.Length; i++)
        {
            if (TryLoseByRemoving(mask, i, currentPlayerWins, nums))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryLoseByRemoving(int mask, int i, Func<int, bool> currentPlayerWins, int[] nums)
    {
        var bit = 1 << i;
        if ((mask & bit) == 0)
        {
            return false;
        }

        var remaining = mask & ~bit;
        return XorOf(remaining, nums) != 0 && !currentPlayerWins(remaining);
    }

    private static int XorOf(int mask, int[] nums)
    {
        var result = 0;
        for (var i = 0; i < nums.Length; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                result ^= nums[i];
            }
        }

        return result;
    }
}
