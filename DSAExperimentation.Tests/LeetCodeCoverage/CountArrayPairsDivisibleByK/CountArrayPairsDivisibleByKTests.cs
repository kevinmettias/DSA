using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountArrayPairsDivisibleByK;

// LeetCode 2183. Count Array Pairs Divisible by K: whether nums[i] * nums[j] is
// divisible by k depends only on g_i = Gcd(nums[i], k) and g_j = Gcd(nums[j], k), not
// on nums[i]/nums[j] themselves - so grouping every index by its own Gcd(nums[i], k)
// via this repo's own HashMap<TKey,TValue> (the same "count occurrences per computed
// key" idiom TwoSumTests uses, just keyed by a derived gcd instead of a raw value) and
// then checking each pair of GROUPS - not each pair of elements - for
// (g_i * g_j) % k == 0 counts every qualifying pair in one pass over the (small,
// divisor-of-k-bounded) distinct keys instead of the O(n^2) elementwise scan. Gcd
// itself is the same private Euclidean-algorithm helper FindGreatestCommonDivisorOfArrayTests/
// GCDSortOfAnArrayTests/CheckIfItIsAGoodArrayTests already reuse inline rather than
// promoting to a shared production type.
public sealed partial class CountArrayPairsDivisibleByKTests
{
    [Fact]
    public void CountPairs_ClassicExampleOne_ReturnsSeven()
    {
        int[] nums = [1, 2, 3, 4, 5];

        var count = CountPairs(nums, k: 2);

        Assert.Equal(7, count);
    }

    [Fact]
    public void CountPairs_ClassicExampleTwo_ReturnsZero()
    {
        int[] nums = [1, 2, 3, 4];

        var count = CountPairs(nums, k: 5);

        Assert.Equal(0, count);
    }

    [Fact]
    public void CountPairs_AllElementsEqualToK_EveryPairQualifies()
    {
        int[] nums = [2, 2, 2, 2];

        var count = CountPairs(nums, k: 2);

        Assert.Equal(6, count);
    }

    [Fact]
    public void CountPairs_KIsOne_EveryPairQualifies()
    {
        int[] nums = [3, 5, 7];

        var count = CountPairs(nums, k: 1);

        Assert.Equal(3, count);
    }

    private static long CountPairs(int[] nums, int k)
    {
        var groupCounts = new HashMap<int, int>();

        foreach (var num in nums)
        {
            var group = Gcd(num, k);
            groupCounts.TryGetValue(group, out var existing);
            groupCounts.Set(group, existing + 1);
        }

        return CountQualifyingGroupPairs(groupCounts, k);
    }

    private static long CountQualifyingGroupPairs(HashMap<int, int> groupCounts, int k)
    {
        var groups = groupCounts.Keys.ToList();
        long pairs = 0;

        for (var i = 0; i < groups.Count; i++)
        {
            for (var j = i; j < groups.Count; j++)
            {
                if ((long)groups[i] * groups[j] % k != 0)
                {
                    continue;
                }

                groupCounts.TryGetValue(groups[i], out var countI);
                groupCounts.TryGetValue(groups[j], out var countJ);

                pairs += i == j ? (long)countI * (countI - 1) / 2 : (long)countI * countJ;
            }
        }

        return pairs;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
