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
    public static bool[] PathExistenceQueriesByBruteForceBfs(int nodeCount, int[] nums, int maxDiff, int[][] queries)
    {
        var adjacency = BuildAdjacency(nodeCount, nums, maxDiff);
        var answers = new bool[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            answers[i] = IsReachableByBfs(adjacency, queries[i][0], queries[i][1]);
        }

        return answers;
    }

    private static List<int>[] BuildAdjacency(int nodeCount, int[] nums, int maxDiff)
    {
        var adjacency = new List<int>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            adjacency[i] = [];
        }

        for (var i = 1; i < nodeCount; i++)
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

        return HasPathToTarget(adjacency, frontier, visited, target);
    }

    // The BFS itself: every dequeued node's neighbours are checked against the target, and a
    // neighbour not yet visited joins the frontier. A drained frontier means no path exists.
    private static bool HasPathToTarget(
        List<int>[] adjacency, Queue<int> frontier, HashSet<int> visited, int target)
    {
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
    public static bool[] PathExistenceQueriesByDisjointSet(int nodeCount, int[] nums, int maxDiff, int[][] queries)
    {
        var groups = ProximityGroups.Build(nodeCount, nums, maxDiff);
        return PathExistenceQueriesByDisjointSet(groups, queries);
    }

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
