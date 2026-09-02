using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumIncompatibility;

// LeetCode 1681. Minimum Incompatibility: bitmask DP over "elements still needing
// a group," memoized by this repo's own Memoizer<TState,TResult> (state = the
// remaining mask, the same shape StoneGameVTests already uses for a (Left,Right)
// state instead) - each step always groups the lowest still-ungrouped index to
// canonicalize which subset gets tried, and this repo's own Set<int> catches "two
// equal values in the same subset" - the problem's own feasibility rule - while a
// candidate group is built.
public sealed partial class MinimumIncompatibilityTests
{
    private const int Infeasible = int.MaxValue / 2;

    [Fact]
    public void MinimumIncompatibility_ClassicExample_SplitsIntoTwoPairs()
    {
        int[] nums = [1, 2, 1, 4];

        var actual = MinimumIncompatibility(nums, k: 2);

        Assert.Equal(4, actual);
    }

    [Fact]
    public void MinimumIncompatibility_FourGroupsOfTwo_ReturnsMinimalSum()
    {
        int[] nums = [6, 3, 8, 1, 3, 1, 2, 2];

        var actual = MinimumIncompatibility(nums, k: 4);

        Assert.Equal(6, actual);
    }

    [Fact]
    public void MinimumIncompatibility_ValueRepeatsMoreThanGroupCount_ReturnsNegativeOne()
    {
        int[] nums = [5, 3, 3, 6, 3, 3];

        var actual = MinimumIncompatibility(nums, k: 3);

        Assert.Equal(-1, actual);
    }

    // Bundles the values that stay fixed across every recursive call in one
    // BestGroupingFor search, so BestSubsetCost only needs (remaining, lowestBit,
    // others, context) instead of six loose parameters.
    private readonly record struct GroupingContext(int[] Nums, int GroupSize, Func<int, int> Rec);

    private static int MinimumIncompatibility(int[] nums, int k)
    {
        var groupSize = nums.Length / k;
        var fullMask = (1 << nums.Length) - 1;

        var best = Memoizer.Memoize<int, int>(
            fullMask, (remaining, rec) => BestGroupingFor(remaining, new GroupingContext(nums, groupSize, rec)));

        return best >= Infeasible ? -1 : best;
    }

    private static int BestGroupingFor(int remaining, GroupingContext context)
    {
        if (remaining == 0)
        {
            return 0;
        }

        var lowestBit = remaining & -remaining;
        var others = remaining & ~lowestBit;

        return BestSubsetCost(remaining, lowestBit, others, context);
    }

    // Tries every subset of size groupSize-1 that, together with the lowest still-
    // ungrouped bit, forms the next group, and recurses on what remains.
    private static int BestSubsetCost(int remaining, int lowestBit, int others, GroupingContext context)
    {
        var bestForState = Infeasible;

        for (var sub = others; ; sub = (sub - 1) & others)
        {
            if (PopCount(sub) == context.GroupSize - 1 && TryGroupCost(context.Nums, sub | lowestBit, out var cost))
            {
                bestForState = Math.Min(bestForState, cost + context.Rec(remaining & ~(sub | lowestBit)));
            }

            if (sub == 0)
            {
                break;
            }
        }

        return bestForState;
    }

    private static bool TryGroupCost(int[] nums, int subset, out int cost)
    {
        var seen = new Set<int>();
        var min = int.MaxValue;
        var max = int.MinValue;

        for (var i = 0; i < nums.Length; i++)
        {
            if ((subset & (1 << i)) == 0)
            {
                continue;
            }

            if (!TryIncludeElement(nums[i], seen, ref min, ref max))
            {
                cost = 0;
                return false;
            }
        }

        cost = max - min;
        return true;
    }

    private static bool TryIncludeElement(int value, Set<int> seen, ref int min, ref int max)
    {
        if (!seen.TryAdd(value))
        {
            return false;
        }

        min = Math.Min(min, value);
        max = Math.Max(max, value);
        return true;
    }

    private static int PopCount(int value)
    {
        var count = 0;

        while (value != 0)
        {
            value &= value - 1;
            count++;
        }

        return count;
    }
}
