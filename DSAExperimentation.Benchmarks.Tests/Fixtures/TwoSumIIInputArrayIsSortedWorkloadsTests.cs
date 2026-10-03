using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for TwoSumIIInputArrayIsSortedWorkloads (ARCHITECTURE 17.7). The reading depends on
// the array being one LC 167 could pose - non-decreasing, every value and the target inside
// [-1000, 1000] - and on its guarantee of exactly one solution, which has to be the last two positions
// for both strategies to walk the whole array. Pairs are counted here by value frequency, without
// either strategy, at both of TwoSumIIInputArrayIsSortedBenchmarks' lengths and at LC 167's own
// 3 * 10^4, where the climb has to repeat values.
public sealed partial class TwoSumIIInputArrayIsSortedWorkloadsTests
{
    private const int SmallestLength = 200;
    private const int LargestBenchmarkLength = 5_000;
    private const int MaxLength = 30_000;

    private const int MinValue = -1_000;
    private const int MaxValue = 1_000;
    private const int ExpectedPairCount = 1;

    // The planted pair is the array's last two values.
    private const int PlantedPairLength = 2;

    public static TheoryData<int> Lengths => new([SmallestLength, LargestBenchmarkLength, MaxLength]);

    [Theory]
    [MemberData(nameof(Lengths))]
    public void BuildNums_Lengths_AreNonDecreasingInsideTheValueRange(int length)
    {
        var nums = TwoSumIIInputArrayIsSortedWorkloads.BuildNums(length);

        Assert.Equal(length, nums.Length);
        Assert.All(nums, value => Assert.InRange(value, MinValue, MaxValue));
        Assert.Equal(nums.Order(), nums);
        Assert.InRange(TwoSumIIInputArrayIsSortedWorkloads.Target, MinValue, MaxValue);
    }

    [Theory]
    [MemberData(nameof(Lengths))]
    public void BuildNums_Lengths_HoldExactlyOnePairSummingToTargetAtTheEnd(int length)
    {
        var nums = TwoSumIIInputArrayIsSortedWorkloads.BuildNums(length);

        Assert.Equal(ExpectedPairCount, PairsSummingToTarget(nums));
        Assert.Equal(TwoSumIIInputArrayIsSortedWorkloads.Target, nums.TakeLast(PlantedPairLength).Sum());
    }

    // Walks the values once, counting for each one how many earlier values complete it to the target.
    private static int PairsSummingToTarget(int[] nums)
    {
        var seen = new Dictionary<int, int>();
        var pairs = 0;

        foreach (var value in nums)
        {
            pairs += seen.GetValueOrDefault(TwoSumIIInputArrayIsSortedWorkloads.Target - value);
            seen[value] = seen.GetValueOrDefault(value) + 1;
        }

        return pairs;
    }
}
