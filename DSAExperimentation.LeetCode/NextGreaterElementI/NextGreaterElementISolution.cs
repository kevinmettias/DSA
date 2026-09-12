using DSAExperimentation.DataStructures.HashMap;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.NextGreaterElementI;

// LeetCode 496. Next Greater Element I: for each value in nums1, find its next
// greater element to the right in nums2, or -1 if none exists.
//
// The two strategies differ in how they answer that per-query lookup: rescanning
// nums2 from scratch for every value in nums1, or a single monotonic-decreasing
// sweep through nums2 that records every value's next-greater element once, into
// this repo's own HashMap<int,int>, so nums1's answers become O(1) lookups.
internal static class NextGreaterElementISolution
{
    // The textbook per-query rescan: for each nums1 value, find it in nums2 and
    // walk right until a larger value turns up. O(n*m); the arm the sweep below
    // has to justify itself against.
    public static int[] NextGreaterElementByPerQueryRescan(int[] nums1, int[] nums2)
    {
        var result = new int[nums1.Length];

        for (var i = 0; i < nums1.Length; i++)
        {
            var position = Array.IndexOf(nums2, nums1[i]);
            var answer = LeetCodeAnswer.None;

            for (var j = position + 1; j < nums2.Length; j++)
            {
                if (nums2[j] > nums1[i])
                {
                    answer = nums2[j];
                    break;
                }
            }

            result[i] = answer;
        }

        return result;
    }

    // A monotonic-decreasing Stack<int> walk over nums2 builds each value's
    // next-greater element in one O(n) pass, recorded into a HashMap<int,int>;
    // nums1's answers then become O(1) lookups into that map, for O(n+m) overall.
    public static int[] NextGreaterElementByMonotonicStackSweep(int[] nums1, int[] nums2)
    {
        var nextGreater = new HashMap<int, int>();
        var decreasing = new RepoIntStack();

        foreach (var value in nums2)
        {
            while (decreasing.TryPeek(out var top) && top < value)
            {
                decreasing.TryPop(out _);
                nextGreater.Set(top, value);
            }

            decreasing.Push(value);
        }

        var result = new int[nums1.Length];

        for (var i = 0; i < nums1.Length; i++)
        {
            result[i] = nextGreater.TryGetValue(nums1[i], out var greater) ? greater : LeetCodeAnswer.None;
        }

        return result;
    }
}
