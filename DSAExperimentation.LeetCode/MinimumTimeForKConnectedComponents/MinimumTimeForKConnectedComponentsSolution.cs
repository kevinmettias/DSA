using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.MinimumTimeForKConnectedComponents;

// LeetCode 3608. Minimum Time for K Connected Components: removing every edge with
// time <= t only ever loses edges as t grows, so the component count is monotone
// non-decreasing in t. Reading that the other way round - adding edges back in
// DEscending time order, starting from n isolated components - the answer is the
// time of whichever union first drops the running component count below k, since
// that is exactly the point where keeping only strictly-larger-time edges no longer
// reaches k components on its own.
//
// Both strategies compute the same monotone predicate; they differ only in how they
// search it: probing candidate times one at a time with a fresh union-find rebuilt
// from scratch each probe (baseline), or a single descending pass that builds the
// answer incrementally (composed).
internal static class MinimumTimeForKConnectedComponentsSolution
{
    // The textbook answer: binary search over each candidate removal time - the
    // distinct edge times plus 0 - rebuilding a plain BCL union-find array from
    // scratch on every probe. Deliberately written without this repo's DisjointSet -
    // the arm the composed solution below has to justify itself against.
    public static int MinTimeByBinarySearchUnionFind(int n, int[][] edges, int k)
    {
        var candidates = new List<int> { 0 };
        candidates.AddRange(edges.Select(edge => edge[2]));
        candidates.Sort();

        var low = 0;
        var high = candidates.Count - 1;

        while (low < high)
        {
            var mid = low + (high - low) / 2;

            if (ComponentsKeepingEdgesAfter(n, edges, candidates[mid]) >= k)
            {
                high = mid;
            }
            else
            {
                low = mid + 1;
            }
        }

        return candidates[low];
    }

    private static int ComponentsKeepingEdgesAfter(int n, int[][] edges, int t)
    {
        var parent = new int[n];

        for (var i = 0; i < n; i++)
        {
            parent[i] = i;
        }

        var components = n;

        foreach (var edge in edges)
        {
            if (TryUnionKeptEdge(parent, edge, t))
            {
                components--;
            }
        }

        return components;
    }

    // One edge of the rebuild: an edge already gone at time `t` joins nothing, and
    // neither does one whose endpoints already share a component; otherwise the two
    // components it connects become one.
    private static bool TryUnionKeptEdge(int[] parent, int[] edge, int t)
    {
        if (edge[2] <= t)
        {
            return false;
        }

        var rootU = Find(parent, edge[0]);
        var rootV = Find(parent, edge[1]);

        if (rootU == rootV)
        {
            return false;
        }

        parent[rootU] = rootV;

        return true;
    }

    private static int Find(int[] parent, int x)
    {
        while (parent[x] != x)
        {
            parent[x] = parent[parent[x]];
            x = parent[x];
        }

        return x;
    }

    // This repo's own DisjointSet, fed edges in descending time order: the component
    // count starts at n and only ever falls as edges are unioned back in, so the
    // answer is the time of whichever union first drops it below k.
    public static int MinTimeByDescendingUnionFind(int n, int[][] edges, int k)
    {
        var disjointSet = new DisjointSet(n);
        var components = n;

        foreach (var edge in edges.OrderByDescending(edge => edge[2]))
        {
            if (disjointSet.IsConnected(edge[0], edge[1]))
            {
                continue;
            }

            disjointSet.Union(edge[0], edge[1]);
            components--;

            if (components < k)
            {
                return edge[2];
            }
        }

        return 0;
    }
}
