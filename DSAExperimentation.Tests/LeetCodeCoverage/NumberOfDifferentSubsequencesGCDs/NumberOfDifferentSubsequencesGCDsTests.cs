using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfDifferentSubsequencesGCDs;

// LeetCode 1819. Number of Different Subsequences GCDs: mark every value's presence
// in this repo's own Set<int> (O(1) Has, the same membership role
// XOfAKindInADeckOfCardsTests' HashMap plays for counts), then for each candidate g
// from 1 to max(nums) fold Gcd over only g's multiples that are present - g is an
// achievable subsequence GCD exactly when that running fold lands back on g itself.
// Walking multiples instead of the whole array per candidate is what turns this into
// O(max log max) (harmonic series) instead of O(max * n); see
// NumberOfDifferentSubsequencesGCDsBenchmarks for that comparison. Same private
// Euclidean Gcd helper CheckIfItIsAGoodArrayTests/XOfAKindInADeckOfCardsTests already
// reuse inline rather than promoting to a shared production type.
public sealed partial class NumberOfDifferentSubsequencesGCDsTests
{
    [Fact]
    public void CountDifferentSubsequenceGcds_LeetCodeExampleOne_ReturnsFive()
    {
        int[] nums = [6, 10, 3];

        Assert.Equal(5, CountDifferentSubsequenceGcds(nums));
    }

    [Fact]
    public void CountDifferentSubsequenceGcds_LeetCodeExampleTwo_ReturnsSeven()
    {
        int[] nums = [5, 15, 40, 5, 6];

        Assert.Equal(7, CountDifferentSubsequenceGcds(nums));
    }

    [Fact]
    public void CountDifferentSubsequenceGcds_SingleValue_ReturnsOne()
    {
        int[] nums = [7];

        Assert.Equal(1, CountDifferentSubsequenceGcds(nums));
    }

    private static int CountDifferentSubsequenceGcds(int[] nums)
    {
        var (present, maxValue) = CollectPresentValues(nums);
        return CountAchievableGcds(present, maxValue);
    }

    private static (Set<int> Present, int MaxValue) CollectPresentValues(int[] nums)
    {
        var present = new Set<int>();
        var maxValue = 0;

        foreach (var num in nums)
        {
            present.TryAdd(num);
            maxValue = Math.Max(maxValue, num);
        }

        return (present, maxValue);
    }

    private static int CountAchievableGcds(Set<int> present, int maxValue)
    {
        var count = 0;

        for (var candidate = 1; candidate <= maxValue; candidate++)
        {
            if (IsAchievableGcd(candidate, maxValue, present))
            {
                count++;
            }
        }

        return count;
    }

    private static bool IsAchievableGcd(int candidate, int maxValue, Set<int> present)
    {
        var runningGcd = 0;

        for (var multiple = candidate; multiple <= maxValue; multiple += candidate)
        {
            if (!present.Has(multiple))
            {
                continue;
            }

            runningGcd = Gcd(runningGcd, multiple);

            if (runningGcd == candidate)
            {
                return true;
            }
        }

        return false;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
