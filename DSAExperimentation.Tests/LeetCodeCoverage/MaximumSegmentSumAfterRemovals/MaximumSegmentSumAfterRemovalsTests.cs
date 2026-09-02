using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSegmentSumAfterRemovals;

// LeetCode 2382. Maximum Segment Sum After Removals: replay removeQueries
// backward as INSERTIONS into an initially-all-removed array, unioning each
// newly-restored index with any already-standing neighbor via this repo's own
// DisjointSet - BricksFallingWhenHitTests' own reverse-time trick, with a
// per-root running SUM threaded beside Find/Union instead of that test's per-root
// SIZE. Because insertion only ever grows or merges segments, the best sum seen so
// far can never shrink as more indices come back, so a single running maximum
// updated after each insertion reconstructs answer[] once the pass is reversed
// back into forward order.
public sealed partial class MaximumSegmentSumAfterRemovalsTests
{
    [Fact]
    public void MaxSegmentSums_ClassicExample_MatchesRunningMaximumAfterEachRemoval()
    {
        int[] nums = [1, 2, 5, 6, 1];
        int[] removeQueries = [0, 3, 2, 4, 1];

        var answer = MaxSegmentSums(nums, removeQueries);

        Assert.Equal([14, 7, 2, 2, 0], answer);
    }

    [Fact]
    public void MaxSegmentSums_SecondExample_MatchesRunningMaximumAfterEachRemoval()
    {
        int[] nums = [3, 2, 11, 1];
        int[] removeQueries = [3, 2, 1, 0];

        var answer = MaxSegmentSums(nums, removeQueries);

        Assert.Equal([16, 5, 3, 0], answer);
    }

    [Fact]
    public void MaxSegmentSums_SingleElement_RemovingItLeavesNothing()
    {
        int[] nums = [42];
        int[] removeQueries = [0];

        var answer = MaxSegmentSums(nums, removeQueries);

        Assert.Equal([0], answer);
    }

    private static long[] MaxSegmentSums(int[] nums, int[] removeQueries)
    {
        var n = nums.Length;
        var answer = new long[n];
        var present = new bool[n];
        var sum = new long[n];
        var components = new DisjointSet(n);
        var maxSum = 0L;

        for (var i = n - 1; i >= 1; i--)
        {
            var index = removeQueries[i];
            Insert(components, present, sum, nums, index);
            maxSum = Math.Max(maxSum, sum[components.Find(index)]);
            answer[i - 1] = maxSum;
        }

        return answer;
    }

    private static void Insert(DisjointSet components, bool[] present, long[] sum, int[] nums, int index)
    {
        present[index] = true;
        sum[index] = nums[index];

        if (index > 0 && present[index - 1])
        {
            Merge(components, sum, index, index - 1);
        }

        if (index < nums.Length - 1 && present[index + 1])
        {
            Merge(components, sum, index, index + 1);
        }
    }

    private static void Merge(DisjointSet components, long[] sum, int first, int second)
    {
        var firstRoot = components.Find(first);
        var secondRoot = components.Find(second);

        components.Union(first, second);
        var mergedRoot = components.Find(first);
        sum[mergedRoot] = sum[firstRoot] + sum[secondRoot];
    }
}
