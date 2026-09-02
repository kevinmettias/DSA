using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.PathExistenceQueriesInAGraphI;

// LeetCode 3532. Path Existence Queries in a Graph I: nums is sorted, and an
// undirected edge joins i and j whenever |nums[i] - nums[j]| <= maxDiff. As
// ProximityGroups' own doc comment works out, that whole graph's connectivity is
// captured by unioning only its n-1 adjacent pairs - the puzzle reduces to one
// equivalence-class query per pair in queries, exactly DisjointSet's own reason
// for existing.
internal static class PathExistenceQueriesInAGraphISolution
{
    // Baseline: the same O(n) reduced adjacency (a genuine O(n^2) all-pairs scan
    // would make even modest benchmark sizes unusable) but answered with a plain
    // BCL queue BFS run fresh for every query - "what you'd write without this
    // repo," recomputing reachability from scratch instead of precomputing it
    // once via DisjointSet.
    public static bool[] PathExistenceQueriesByBruteForceBfs(int n, int[] nums, int maxDiff, int[][] queries)
    {
        var adjacency = BuildAdjacency(n, nums, maxDiff);
        var answers = new bool[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            answers[i] = IsReachableByBfs(adjacency, queries[i][0], queries[i][1]);
        }

        return answers;
    }

    private static List<int>[] BuildAdjacency(int n, int[] nums, int maxDiff)
    {
        var adjacency = new List<int>[n];

        for (var i = 0; i < n; i++)
        {
            adjacency[i] = [];
        }

        for (var i = 1; i < n; i++)
        {
            if (nums[i] - nums[i - 1] <= maxDiff)
            {
                adjacency[i - 1].Add(i);
                adjacency[i].Add(i - 1);
            }
        }

        return adjacency;
    }

    private static bool IsReachableByBfs(List<int>[] adjacency, int source, int target)
    {
        if (source == target)
        {
            return true;
        }

        var visited = new HashSet<int> { source };
        var frontier = new Queue<int>();
        frontier.Enqueue(source);

        while (frontier.Count > 0)
        {
            var node = frontier.Dequeue();

            foreach (var neighbor in adjacency[node])
            {
                if (neighbor == target)
                {
                    return true;
                }

                if (visited.Add(neighbor))
                {
                    frontier.Enqueue(neighbor);
                }
            }
        }

        return false;
    }

    // Composed: ProximityGroups unions every adjacent pair once via
    // DataStructures.DisjointSet; every query after that is a single
    // IsConnected check.
    public static bool[] PathExistenceQueriesByDisjointSet(int n, int[] nums, int maxDiff, int[][] queries) =>
        PathExistenceQueriesByDisjointSet(ProximityGroups.Build(n, nums, maxDiff), queries);

    public static bool[] PathExistenceQueriesByDisjointSet(DisjointSet groups, int[][] queries)
    {
        var answers = new bool[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            answers[i] = groups.IsConnected(queries[i][0], queries[i][1]);
        }

        return answers;
    }
}
