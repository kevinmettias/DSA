using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumTimeToMakeArraySumAtMostX;

// LeetCode 2809. Minimum Time to Make Array Sum At Most x: pair each nums1[i] with its
// nums2[i] and sort the pairs ascending by nums2 with this repo's own MergeSort (over an
// ArrayIndexedSequence<(int,int)>, the same tuple-sort idiom MaximumSumQueriesTests uses)
// - the standard exchange argument for this problem is that zeroing an index at time
// slot j is worth nums1[i] + nums2[i]*j, and the optimal schedule always assigns the
// smallest nums2 to the earliest slots. A 0/1-knapsack-shaped DP over "how many indices
// have been zeroed so far" then finds, for every possible operation count t, the maximum
// total reduction reachable by zeroing exactly t of the sorted indices in slot order.
// sum(nums1) + t*sum(nums2) - reduction[t] is nums1's total after t seconds under that
// optimal schedule, so the first t for which it's <= x is the answer.
public sealed partial class MinimumTimeToMakeArraySumAtMostXTests
{
    [Fact]
    public void MinimumTime_LeetCodeExampleOne_ReturnsThreeSeconds()
    {
        int[] nums1 = [1, 2, 3];
        int[] nums2 = [1, 2, 3];

        Assert.Equal(3, MinimumTime(nums1, nums2, 4));
    }

    [Fact]
    public void MinimumTime_LeetCodeExampleTwo_TargetUnreachableReturnsNegativeOne()
    {
        int[] nums1 = [1, 2, 3];
        int[] nums2 = [3, 3, 3];

        Assert.Equal(-1, MinimumTime(nums1, nums2, 4));
    }

    [Fact]
    public void MinimumTime_SumAlreadyAtMostX_ReturnsZero()
    {
        int[] nums1 = [5];
        int[] nums2 = [1];

        Assert.Equal(0, MinimumTime(nums1, nums2, 10));
    }

    private static int MinimumTime(int[] nums1, int[] nums2, int x)
    {
        var n = nums1.Length;
        var pairs = BuildPairsAscendingByNums2(nums1, nums2);
        var reduction = MaxReductionForOperationCount(pairs);

        long sum1 = 0;
        long sum2 = 0;

        foreach (var value in nums1)
        {
            sum1 += value;
        }

        foreach (var value in nums2)
        {
            sum2 += value;
        }

        for (var t = 0; t <= n; t++)
        {
            if (sum1 + (sum2 * t) - reduction[t] <= x)
            {
                return t;
            }
        }

        return -1;
    }

    private static (int Nums1, int Nums2)[] BuildPairsAscendingByNums2(int[] nums1, int[] nums2)
    {
        var pairs = new (int Nums1, int Nums2)[nums1.Length];

        for (var i = 0; i < nums1.Length; i++)
        {
            pairs[i] = (nums1[i], nums2[i]);
        }

        var byNums2Ascending = Comparer<(int Nums1, int Nums2)>.Create((a, b) => a.Nums2.CompareTo(b.Nums2));
        MergeSort.Sort<(int Nums1, int Nums2), ArrayIndexedSequence<(int Nums1, int Nums2)>>(
            new ArrayIndexedSequence<(int Nums1, int Nums2)>(pairs), byNums2Ascending);

        return pairs;
    }

    // dp[j] is the max total reduction achievable by zeroing exactly j of the indices seen so
    // far, each assigned to the earliest still-open slot in sorted (ascending nums2) order - the
    // textbook backward 0/1-knapsack iteration, where the "weight" spent per item is always 1 and
    // the slot index j doubles as both the DP index and the item's assigned time slot.
    private static long[] MaxReductionForOperationCount((int Nums1, int Nums2)[] pairs)
    {
        var n = pairs.Length;
        var dp = new long[n + 1];

        for (var i = 0; i < n; i++)
        {
            var (a1, a2) = pairs[i];

            for (var j = Math.Min(i + 1, n); j >= 1; j--)
            {
                dp[j] = Math.Max(dp[j], dp[j - 1] + a1 + ((long)a2 * j));
            }
        }

        return dp;
    }
}
