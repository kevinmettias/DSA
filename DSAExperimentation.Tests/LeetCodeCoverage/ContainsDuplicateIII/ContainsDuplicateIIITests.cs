using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ContainsDuplicateIII;

// LeetCode 220. Contains Duplicate III: bucket the sliding window of the last indexDiff
// elements by value/(valueDiff+1), using this repo's own HashMap<long,long> (long-keyed so
// nums[i]-lower/upper never overflows int at the INT_MIN/INT_MAX extremes the last test case
// below exercises). At most one value can occupy a given bucket while a match is still
// possible, so checking the same bucket plus its two immediate neighbors is enough to prove
// |nums[i]-nums[j]| <= valueDiff without a sorted structure.
public sealed partial class ContainsDuplicateIIITests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3, 1 }, 3, 0, true)]
    [InlineData(new[] { 1, 0, 1, 1 }, 1, 2, true)]
    [InlineData(new[] { 1, 5, 9, 1, 5, 9 }, 2, 3, false)]
    [InlineData(new[] { -1, -1 }, 1, 0, true)]
    [InlineData(new[] { -1, 2147483647 }, 1, 2147483647, false)]
    public void ContainsNearbyAlmostDuplicate_Examples_ReturnsExpected(int[] nums, int indexDiff, int valueDiff, bool expected)
        => Assert.Equal(expected, ContainsNearbyAlmostDuplicate(nums, indexDiff, valueDiff));

    private static bool ContainsNearbyAlmostDuplicate(int[] nums, int indexDiff, int valueDiff)
    {
        if (indexDiff <= 0 || valueDiff < 0)
        {
            return false;
        }

        var width = (long)valueDiff + 1;
        var buckets = new HashMap<long, long>();

        for (var i = 0; i < nums.Length; i++)
        {
            var bucketId = BucketId(nums[i], width);

            if (buckets.HasKey(bucketId)
                || (buckets.TryGetValue(bucketId - 1, out var lower) && nums[i] - lower <= valueDiff)
                || (buckets.TryGetValue(bucketId + 1, out var upper) && upper - nums[i] <= valueDiff))
            {
                return true;
            }

            buckets.Set(bucketId, nums[i]);

            if (i >= indexDiff)
            {
                buckets.TryRemove(BucketId(nums[i - indexDiff], width));
            }
        }

        return false;
    }

    private static long BucketId(long value, long width) => value >= 0 ? value / width : ((value + 1) / width) - 1;
}
