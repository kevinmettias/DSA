using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.JumpGameIX;

// LeetCode 3660. Jump Game IX: from index i you may jump forward (j > i) only to a
// smaller value, or backward (j < i) only to a larger value. For each i, find the
// maximum value reachable by any sequence of such jumps.
//
// The jump rules are reciprocal: whenever i can jump forward to j (nums[j] <
// nums[i]), j can jump straight back to i (nums[i] > nums[j]) - and symmetrically
// for a backward jump. So "i < j with nums[i] > nums[j]" is really one undirected
// edge, and the answer for every index is just the maximum value in its connected
// component. Consecutive indices i, i+1 land in different components exactly where
// a valid split point exists - max(nums[0..i]) <= min(nums[i+1..n-1]) - so only
// those n - 1 boundaries, not all O(n^2) inversions, need to be checked to recover
// every component with a union-find.
internal static class JumpGameIXSolution
{
    // Textbook baseline: implements the two jump rules literally and BFS-explores
    // every index's reachable set by scanning all other indices at each step,
    // without ever using the fact that the graph turns out to be undirected. O(n^3)
    // worst case - the arm the union-find strategy below has to justify itself
    // against.
    public static int[] MaxValuesByJumpBfs(int[] nums)
    {
        var n = nums.Length;
        var answer = new int[n];

        for (var start = 0; start < n; start++)
        {
            answer[start] = MaxReachableFrom(nums, start);
        }

        return answer;
    }

    private static int MaxReachableFrom(int[] nums, int start)
    {
        var n = nums.Length;
        var visited = new bool[n];
        var queue = new Queue<int>();
        visited[start] = true;
        queue.Enqueue(start);
        var best = nums[start];

        while (queue.Count > 0)
        {
            var i = queue.Dequeue();
            best = Math.Max(best, nums[i]);

            for (var j = 0; j < n; j++)
            {
                if (!visited[j] && CanJump(nums, i, j))
                {
                    visited[j] = true;
                    queue.Enqueue(j);
                }
            }
        }

        return best;
    }

    private static bool CanJump(int[] nums, int i, int j) =>
        (j > i && nums[j] < nums[i]) || (j < i && nums[j] > nums[i]);

    // Composed: union index i with i + 1 wherever the prefix-max/suffix-min split
    // test fails at that boundary, using this repo's DisjointSet - the same
    // amortized-inverse-Ackermann Find/Union
    // MakeLexicographicallySmallestArrayBySwappingElements already composes for its
    // own grouping. n - 1 unions instead of the O(n^2) all-inversions graph, then
    // one pass to read off each component's max.
    public static int[] MaxValuesByAdjacentUnionFind(int[] nums)
    {
        var n = nums.Length;
        var prefixMax = new int[n];
        var suffixMin = new int[n];

        prefixMax[0] = nums[0];
        for (var i = 1; i < n; i++)
        {
            prefixMax[i] = Math.Max(prefixMax[i - 1], nums[i]);
        }

        suffixMin[n - 1] = nums[n - 1];
        for (var i = n - 2; i >= 0; i--)
        {
            suffixMin[i] = Math.Min(suffixMin[i + 1], nums[i]);
        }

        var components = new DisjointSet(n);
        for (var i = 0; i < n - 1; i++)
        {
            if (prefixMax[i] > suffixMin[i + 1])
            {
                components.Union(i, i + 1);
            }
        }

        var maxByRoot = new Dictionary<int, int>();
        for (var i = 0; i < n; i++)
        {
            var root = components.Find(i);
            maxByRoot[root] = maxByRoot.TryGetValue(root, out var current) ? Math.Max(current, nums[i]) : nums[i];
        }

        var answer = new int[n];
        for (var i = 0; i < n; i++)
        {
            answer[i] = maxByRoot[components.Find(i)];
        }

        return answer;
    }
}
