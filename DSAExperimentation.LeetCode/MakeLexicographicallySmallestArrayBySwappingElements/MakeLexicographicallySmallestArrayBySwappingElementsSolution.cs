using DisjointSetOperations = DSAExperimentation.DataStructures.DisjointSet.DisjointSet;

namespace DSAExperimentation.LeetCode.MakeLexicographicallySmallestArrayBySwappingElements;

// LeetCode 2948. Make Lexicographically Smallest Array by Swapping Elements: two
// indices are swappable, transitively, whenever a chain of values each within
// `limit` of the next connects them. Sorting by value turns that chain into
// adjacency - two sorted-order neighbors belong to the same swappable group
// exactly when their values differ by at most `limit` - and within a connected
// group every permutation of its values is reachable, so the lexicographically
// smallest array assigns each group's values, smallest first, to that group's
// original indices, smallest first.
//
// Both strategies sort once and assign identically; they differ only in how a
// group's membership is decided while scanning the sorted order.
internal static class MakeLexicographicallySmallestArrayBySwappingElementsSolution
{
    // The textbook form: sorted adjacency alone decides where one group ends and
    // the next begins, so no explicit set-membership structure is needed - just
    // watch for the chain breaking as the scan proceeds.
    public static int[] LexicographicallySmallestArrayByContiguousGroups(int[] nums, int limit)
    {
        var order = SortIndicesByValue(nums);
        var result = new int[nums.Length];
        var groupStart = 0;

        for (var k = 0; k < order.Length; k++)
        {
            var chainBroken = k == order.Length - 1 || nums[order[k + 1]] - nums[order[k]] > limit;
            if (!chainBroken)
            {
                continue;
            }

            AssignGroup(nums, order, (groupStart, k), result);
            groupStart = k + 1;
        }

        return result;
    }

    // Same scan, but group membership is asked of this repo's own DisjointSet
    // rather than inferred from the chain directly: adjacent sorted values within
    // `limit` are unioned, and a group boundary is wherever two sorted-order
    // neighbors land in different components.
    public static int[] LexicographicallySmallestArrayByDisjointSet(int[] nums, int limit)
    {
        var order = SortIndicesByValue(nums);
        var groups = new DisjointSetOperations(nums.Length);

        for (var k = 0; k < order.Length - 1; k++)
        {
            if (nums[order[k + 1]] - nums[order[k]] <= limit)
            {
                groups.Union(order[k], order[k + 1]);
            }
        }

        var result = new int[nums.Length];
        AssignGroupsByComponent(nums, order, groups, result);

        return result;
    }

    // Scans the sorted order and writes each group into the result as it ends: a
    // group ends where two sorted-order neighbours fall in different components of
    // the union-find built above, or where the sorted order itself runs out.
    private static void AssignGroupsByComponent(
        int[] nums, int[] order, DisjointSetOperations groups, int[] result)
    {
        var groupStart = 0;

        for (var k = 0; k < order.Length; k++)
        {
            var chainBroken = k == order.Length - 1 || !groups.IsConnected(order[k], order[k + 1]);
            if (!chainBroken)
            {
                continue;
            }

            AssignGroup(nums, order, (groupStart, k), result);
            groupStart = k + 1;
        }
    }

    // Original indices in ascending order of nums[index], the shared basis both
    // strategies scan.
    private static int[] SortIndicesByValue(int[] nums)
    {
        var order = new int[nums.Length];

        for (var i = 0; i < order.Length; i++)
        {
            order[i] = i;
        }

        Array.Sort(order, (a, b) => nums[a].CompareTo(nums[b]));
        return order;
    }

    // order[start..end] is one swappable group: its values are already ascending
    // (order is sorted by value), so writing them into its original indices,
    // smallest index first, is the lexicographically smallest assignment for the
    // group.
    private static void AssignGroup(int[] nums, int[] order, (int Start, int End) range, int[] result)
    {
        var indices = new int[range.End - range.Start + 1];
        Array.Copy(order, range.Start, indices, 0, indices.Length);
        Array.Sort(indices);

        for (var offset = 0; offset < indices.Length; offset++)
        {
            result[indices[offset]] = nums[order[range.Start + offset]];
        }
    }
}
