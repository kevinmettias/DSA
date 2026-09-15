using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.ContainsDuplicateIII;

// LeetCode 220. Contains Duplicate III: does the array contain two indices i, j
// with |i-j| <= indexDiff and |nums[i]-nums[j]| <= valueDiff?
//
// The bucketed strategy buckets the sliding window of the last indexDiff elements
// by value/(valueDiff+1), using this repo's own HashMap<long,long> (long-keyed so
// nums[i]-lower/upper never overflows int at the INT_MIN/INT_MAX extremes LeetCode's
// own examples exercise). At most one value can occupy a given bucket while a match
// is still possible, so checking the same bucket plus its two immediate neighbors is
// enough to prove |nums[i]-nums[j]| <= valueDiff without a sorted structure.
internal static class ContainsDuplicateIIISolution
{
    public static bool ContainsNearbyAlmostDuplicateByBucketedHashMap(int[] nums, int indexDiff, int valueDiff)
    {
        if (indexDiff <= 0 || valueDiff < 0)
        {
            return false;
        }

        var buckets = new HashMap<long, long>();

        for (var i = 0; i < nums.Length; i++)
        {
            if (HasNearbyDuplicate(nums, i, (indexDiff, valueDiff), buckets))
            {
                return true;
            }
        }

        return false;
    }

    // indexDiff and valueDiff are adjacent ints of the same type, so only position told
    // them apart at the call site; as one `bounds` argument each says what it measures.
    private static bool HasNearbyDuplicate(
        int[] nums, int i, (int IndexDiff, int ValueDiff) bounds, HashMap<long, long> buckets)
    {
        var (indexDiff, valueDiff) = bounds;
        var width = (long)valueDiff + 1;
        var bucketId = BucketId(nums[i], width);

        if (HasDuplicateWithinValueDiff(bucketId, nums[i], valueDiff, buckets))
        {
            return true;
        }

        buckets.Set(bucketId, nums[i]);

        if (i >= indexDiff)
        {
            var evictedBucketId = BucketId(nums[i - indexDiff], width);
            buckets.TryRemove(evictedBucketId);
        }

        return false;
    }

    private static long BucketId(long value, long width) =>
        value >= 0 ? NonNegativeBucket(value, width) : NegativeBucket(value, width);

    // A non-negative value's bucket: C#'s integer division already floors a
    // non-negative quotient, so the quotient IS the bucket.
    private static long NonNegativeBucket(long value, long width) => value / width;

    // A negative value's bucket, floored: C#'s division truncates toward zero, so the
    // bucket a negative value belongs to sits one below the quotient it truncates to.
    private static long NegativeBucket(long value, long width) => ((value + 1) / width) - 1;

    // The value's own bucket, or either neighbor, can hold a value within valueDiff;
    // a neighbor bucket only qualifies if the pair of values is also close enough,
    // since one bucket's width is exactly valueDiff + 1.
    private static bool HasDuplicateWithinValueDiff(
        long bucketId, long value, int valueDiff, HashMap<long, long> buckets)
        => buckets.HasKey(bucketId)
            || (buckets.TryGetValue(bucketId - 1, out var lower) && value - lower <= valueDiff)
            || (buckets.TryGetValue(bucketId + 1, out var upper) && upper - value <= valueDiff);

    // The canonical O(n*indexDiff) sliding-window brute force: for every index,
    // scan back across the last indexDiff elements directly, no bucketing.
    // Deliberately written without this repo's primitives - a plain int[] and BCL
    // Math - it is the arm the bucketed strategy above has to justify itself
    // against. Note the guard the bucketed strategy needs for indexDiff<=0 or
    // valueDiff<0 is unnecessary here: an empty backward window and an
    // unsatisfiable Math.Abs(...) <= valueDiff comparison already fall out to
    // false on their own.
    public static bool ContainsNearbyAlmostDuplicateBySlidingWindowBruteForce(int[] nums, int indexDiff, int valueDiff)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = Math.Max(0, i - indexDiff); j < i; j++)
            {
                if (Math.Abs((long)nums[i] - nums[j]) <= valueDiff)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
