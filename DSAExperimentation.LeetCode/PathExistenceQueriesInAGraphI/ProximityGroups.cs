using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.PathExistenceQueriesInAGraphI;

// LC 3532's graph never needs materializing its full O(n^2) edge set: nums is
// sorted non-decreasing, so nums[j] - nums[i] (for i < j) is the sum of every
// adjacent gap between them, and every term of that sum is non-negative - so
// whenever a direct edge (i, j) exists (their difference is within maxDiff),
// EVERY adjacent pair strictly between i and j is within maxDiff of its neighbor
// too, since each adjacent gap is at most the total span. Unioning only the n-1
// adjacent pairs whose gap is within maxDiff therefore reaches exactly the same
// components the full all-pairs graph would.
internal static class ProximityGroups
{
    public static DisjointSet Build(int nodeCount, int[] nums, int maxDiff)
    {
        var groups = new DisjointSet(nodeCount);

        for (var i = 1; i < nodeCount; i++)
        {
            if (nums[i] - nums[i - 1] <= maxDiff)
            {
                groups.Union(i - 1, i);
            }
        }

        return groups;
    }
}
