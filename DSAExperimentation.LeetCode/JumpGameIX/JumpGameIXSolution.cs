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
        var (visited, queue) = CreateFrontier(nums.Length, start);
        var best = nums[start];

        while (queue.Count > 0)
        {
            var i = queue.Dequeue();
            best = Math.Max(best, nums[i]);

            for (var j = 0; j < nums.Length; j++)
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

    // The frontier a BFS starts from: the start index alone, already marked visited.
    private static (bool[] Visited, Queue<int> Queue) CreateFrontier(int n, int start)
    {
        var visited = new bool[n];
        var queue = new Queue<int>();
        visited[start] = true;
        queue.Enqueue(start);

        return (visited, queue);
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
        var components = UnionSplitComponents(n, PrefixMaxima(nums), SuffixMinima(nums));

        var maxByRoot = MaxValuePerRoot(nums, components);

        return ReadComponentMaxima(nums, components, maxByRoot);
    }

    // A valid split point sits where max(nums[0..i]) <= min(nums[i+1..n-1]); every
    // boundary that is not one joins i to i + 1 in the same component.
    private static DisjointSet UnionSplitComponents(int n, int[] prefixMax, int[] suffixMin)
    {
        var components = new DisjointSet(n);

        for (var i = 0; i < n - 1; i++)
        {
            if (prefixMax[i] > suffixMin[i + 1])
            {
                components.Union(i, i + 1);
            }
        }

        return components;
    }

    // Running maximum from the left, at every index.
    private static int[] PrefixMaxima(int[] nums)
    {
        var prefixMax = new int[nums.Length];

        prefixMax[0] = nums[0];
        for (var i = 1; i < nums.Length; i++)
        {
            prefixMax[i] = Math.Max(prefixMax[i - 1], nums[i]);
        }

        return prefixMax;
    }

    // Running minimum from the right, at every index.
    private static int[] SuffixMinima(int[] nums)
    {
        var n = nums.Length;
        var suffixMin = new int[n];

        suffixMin[n - 1] = nums[n - 1];
        for (var i = n - 2; i >= 0; i--)
        {
            suffixMin[i] = Math.Min(suffixMin[i + 1], nums[i]);
        }

        return suffixMin;
    }

    // Every component's maximum value, keyed by that component's root.
    private static Dictionary<int, int> MaxValuePerRoot(int[] nums, DisjointSet components)
    {
        var maxByRoot = new Dictionary<int, int>();

        for (var i = 0; i < nums.Length; i++)
        {
            var root = components.Find(i);

            if (maxByRoot.TryGetValue(root, out var current))
            {
                maxByRoot[root] = Math.Max(current, nums[i]);
                continue;
            }

            maxByRoot[root] = nums[i];
        }

        return maxByRoot;
    }

    // Each index's own answer: the maximum value in its component.
    private static int[] ReadComponentMaxima(int[] nums, DisjointSet components, Dictionary<int, int> maxByRoot)
    {
        var answer = new int[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            answer[i] = maxByRoot[components.Find(i)];
        }

        return answer;
    }
}
